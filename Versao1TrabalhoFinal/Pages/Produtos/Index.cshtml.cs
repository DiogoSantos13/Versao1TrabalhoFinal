using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Produtos
{
    /// <summary>
    /// Página de listagem de produtos com filtros, paginação, compra para clientes e gestão para administradores e colaboradores.
    /// </summary>
    [Authorize(Roles = "Cliente,Admin,Colaborador")]
    public class IndexModel : PageModel
    {
        private readonly StandDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Inicializa uma nova instância da página de produtos.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        /// <param name="userManager">Serviço de gestão de utilizadores.</param>
        public IndexModel(StandDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Lista de produtos a apresentar na página atual.
        /// </summary>
        public IList<Produto> Produtos { get; set; } = new List<Produto>();

        /// <summary>
        /// Marca usada no filtro.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? Marca { get; set; }

        /// <summary>
        /// Modelo usado no filtro.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? Modelo { get; set; }

        /// <summary>
        /// Página atual da paginação.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public int PaginaAtual { get; set; } = 1;

        /// <summary>
        /// Quantidade de produtos por página.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public int TamanhoPagina { get; set; } = 8;

        /// <summary>
        /// Número total de produtos encontrados após filtros.
        /// </summary>
        public int TotalProdutos { get; set; }

        /// <summary>
        /// Número total de páginas.
        /// </summary>
        public int TotalPaginas { get; set; }

        /// <summary>
        /// Indica se existe página anterior.
        /// </summary>
        public bool TemPaginaAnterior => PaginaAtual > 1;

        /// <summary>
        /// Indica se existe página seguinte.
        /// </summary>
        public bool TemPaginaSeguinte => PaginaAtual < TotalPaginas;

        /// <summary>
        /// Indica se o utilizador atual tem permissões para gerir produtos.
        /// </summary>
        public bool PodeGerirProdutos =>
            User.IsInRole("Admin") || User.IsInRole("Colaborador");

        /// <summary>
        /// Indica se o utilizador atual pode adicionar produtos ao carrinho.
        /// </summary>
        public bool PodeComprarProdutos =>
            User.IsInRole("Cliente");

        /// <summary>
        /// Carrega a listagem paginada de produtos com filtros opcionais.
        /// </summary>
        public async Task OnGetAsync()
        {
            if (PaginaAtual < 1)
            {
                PaginaAtual = 1;
            }

            var tamanhosPermitidos = new[] { 4, 8, 12, 16, 24 };

            if (!tamanhosPermitidos.Contains(TamanhoPagina))
            {
                TamanhoPagina = 8;
            }

            var query = _context.Produtos
                .Include(p => p.Fornecedor)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(Marca))
            {
                query = query.Where(p => p.MarcaVeiculo != null && p.MarcaVeiculo.Contains(Marca));
            }

            if (!string.IsNullOrWhiteSpace(Modelo))
            {
                query = query.Where(p => p.ModeloVeiculo != null && p.ModeloVeiculo.Contains(Modelo));
            }

            TotalProdutos = await query.CountAsync();

            TotalPaginas = (int)Math.Ceiling(TotalProdutos / (double)TamanhoPagina);

            if (TotalPaginas == 0)
            {
                TotalPaginas = 1;
            }

            if (PaginaAtual > TotalPaginas)
            {
                PaginaAtual = TotalPaginas;
            }

            Produtos = await query
                .OrderBy(p => p.Nome)
                .Skip((PaginaAtual - 1) * TamanhoPagina)
                .Take(TamanhoPagina)
                .ToListAsync();
        }

        /// <summary>
        /// Adiciona um produto ao carrinho do cliente autenticado.
        /// </summary>
        /// <param name="produtoId">Identificador do produto.</param>
        /// <returns>Redirecionamento para a página atual mantendo filtros e paginação.</returns>
        public async Task<IActionResult> OnPostAddAsync(int produtoId)
        {
            if (!User.IsInRole("Cliente"))
            {
                return Forbid();
            }

            var cliente = await ObterClienteAutenticadoAsync();

            if (cliente == null)
            {
                TempData["ErrorMessage"] = "Cliente autenticado não encontrado.";
                return RedirectToPage(new { Marca, Modelo, PaginaAtual, TamanhoPagina });
            }

            var produto = await _context.Produtos
                .FirstOrDefaultAsync(p => p.Id == produtoId);

            if (produto == null)
            {
                TempData["ErrorMessage"] = "Produto não encontrado.";
                return RedirectToPage(new { Marca, Modelo, PaginaAtual, TamanhoPagina });
            }

            if (produto.Stock <= 0)
            {
                TempData["ErrorMessage"] = "Produto sem stock disponível.";
                return RedirectToPage(new { Marca, Modelo, PaginaAtual, TamanhoPagina });
            }

            var carrinho = await _context.Carrinhos
                .FirstOrDefaultAsync(c => c.ClienteId == cliente.Id);

            if (carrinho == null)
            {
                carrinho = new Models.Carrinho
                {
                    ClienteId = cliente.Id,
                    DataCriacao = DateTime.Now
                };

                _context.Carrinhos.Add(carrinho);
                await _context.SaveChangesAsync();
            }

            var itemExistente = await _context.CarrinhoProdutos
                .FirstOrDefaultAsync(cp => cp.CarrinhoId == carrinho.Id && cp.ProdutoId == produto.Id);

            if (itemExistente == null)
            {
                var novoItem = new CarrinhoProdutos
                {
                    CarrinhoId = carrinho.Id,
                    ProdutoId = produto.Id,
                    Quantidade = 1,
                    PrecoNoMomento = produto.Preco,
                    DataAdicao = DateTime.Now
                };

                _context.CarrinhoProdutos.Add(novoItem);
            }
            else
            {
                if (itemExistente.Quantidade < produto.Stock)
                {
                    itemExistente.Quantidade++;
                }
                else
                {
                    TempData["ErrorMessage"] = "Não existe stock suficiente.";
                    return RedirectToPage(new { Marca, Modelo, PaginaAtual, TamanhoPagina });
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Produto adicionado ao carrinho com sucesso.";

            return RedirectToPage(new { Marca, Modelo, PaginaAtual, TamanhoPagina });
        }

        /// <summary>
        /// Obtém o cliente associado ao utilizador autenticado.
        /// </summary>
        /// <returns>Cliente autenticado ou null.</returns>
        private async Task<Cliente?> ObterClienteAutenticadoAsync()
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            return await _context.Clientes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.IdentityUserId == userId);
        }
    }
}