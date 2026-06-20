using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;
using ClienteEntity = Versao1TrabalhoFinal.Models.Cliente;
using CarrinhoEntity = Versao1TrabalhoFinal.Models.Carrinho;

namespace Versao1TrabalhoFinal.Pages.Produtos
{
    /// <summary>
    /// Página de listagem de produtos com filtros, paginação, compra para clientes e gestão para administradores e colaboradores.
    /// </summary>
    [Authorize(Roles = "Cliente,Admin,Colaborador")]
    public class IndexModel : PageModel
    {
        /// <summary>
        /// Contexto da base de dados.
        /// </summary>
        private readonly StandDbContext _context;

        /// <summary>
        /// Serviço de gestão de utilizadores do Identity.
        /// </summary>
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
            // Garante que a página atual nunca é inferior a 1.
            if (PaginaAtual < 1)
            {
                PaginaAtual = 1;
            }

            // Define os tamanhos de paginação permitidos.
            var tamanhosPermitidos = new[] { 4, 8, 12, 16, 24 };

            // Se o tamanho pedido não for válido, usa o valor por defeito.
            if (!tamanhosPermitidos.Contains(TamanhoPagina))
            {
                TamanhoPagina = 8;
            }

            // Prepara a query base dos produtos, incluindo o fornecedor.
            var query = _context.Produtos
                .Include(p => p.Fornecedor)
                .AsNoTracking()
                .AsQueryable();

            // Aplica filtro por marca, se fornecido.
            if (!string.IsNullOrWhiteSpace(Marca))
            {
                query = query.Where(p =>
                    p.MarcaVeiculo != null &&
                    p.MarcaVeiculo.Contains(Marca));
            }

            // Aplica filtro por modelo, se fornecido.
            if (!string.IsNullOrWhiteSpace(Modelo))
            {
                query = query.Where(p =>
                    p.ModeloVeiculo != null &&
                    p.ModeloVeiculo.Contains(Modelo));
            }

            // Conta o total de produtos após filtros.
            TotalProdutos = await query.CountAsync();

            // Calcula o número total de páginas.
            TotalPaginas = (int)Math.Ceiling(TotalProdutos / (double)TamanhoPagina);

            // Garante pelo menos 1 página, mesmo sem resultados.
            if (TotalPaginas == 0)
            {
                TotalPaginas = 1;
            }

            // Se a página atual exceder o limite, ajusta para a última página.
            if (PaginaAtual > TotalPaginas)
            {
                PaginaAtual = TotalPaginas;
            }

            // Obtém apenas os produtos da página atual.
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
            // Apenas clientes podem adicionar produtos ao carrinho.
            if (!User.IsInRole("Cliente"))
            {
                return Forbid();
            }

            // Obtém o cliente autenticado.
            var cliente = await ObterClienteAutenticadoAsync();

            // Se não existir cliente associado, mostra erro e mantém contexto da página.
            if (cliente == null)
            {
                TempData["ErrorMessage"] = "Cliente autenticado não encontrado.";
                return RedirectToPage(new { Marca, Modelo, PaginaAtual, TamanhoPagina });
            }

            // Procura o produto pretendido.
            var produto = await _context.Produtos
                .FirstOrDefaultAsync(p => p.Id == produtoId);

            // Se o produto não existir, mostra erro.
            if (produto == null)
            {
                TempData["ErrorMessage"] = "Produto não encontrado.";
                return RedirectToPage(new { Marca, Modelo, PaginaAtual, TamanhoPagina });
            }

            // Impede adicionar produtos sem stock.
            if (produto.Stock <= 0)
            {
                TempData["ErrorMessage"] = "Produto sem stock disponível.";
                return RedirectToPage(new { Marca, Modelo, PaginaAtual, TamanhoPagina });
            }

            // Tenta obter o carrinho do cliente.
            var carrinho = await _context.Carrinhos
                .FirstOrDefaultAsync(c => c.ClienteId == cliente.Id);

            // Se ainda não existir carrinho, cria um novo.
            if (carrinho == null)
            {
                carrinho = new CarrinhoEntity
                {
                    ClienteId = cliente.Id,
                    DataCriacao = DateTime.Now
                };

                _context.Carrinhos.Add(carrinho);
                await _context.SaveChangesAsync();
            }

            // Verifica se o produto já existe no carrinho.
            var itemExistente = await _context.CarrinhoProdutos
                .FirstOrDefaultAsync(cp =>
                    cp.CarrinhoId == carrinho.Id &&
                    cp.ProdutoId == produto.Id);

            // Se ainda não existir, cria um novo item.
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
                // Se já existir, incrementa a quantidade apenas se houver stock disponível.
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

            // Guarda as alterações do carrinho.
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Produto adicionado ao carrinho com sucesso.";
            return RedirectToPage(new { Marca, Modelo, PaginaAtual, TamanhoPagina });
        }

        /// <summary>
        /// Obtém o cliente associado ao utilizador autenticado.
        /// </summary>
        /// <returns>Cliente autenticado ou null.</returns>
        private async Task<ClienteEntity?> ObterClienteAutenticadoAsync()
        {
            // Obtém o identificador do utilizador autenticado.
            var userId = _userManager.GetUserId(User);

            // Se não existir utilizador autenticado, devolve null.
            if (string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            // Procura o cliente associado ao utilizador autenticado.
            return await _context.Clientes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.IdentityUserId == userId);
        }
    }
}