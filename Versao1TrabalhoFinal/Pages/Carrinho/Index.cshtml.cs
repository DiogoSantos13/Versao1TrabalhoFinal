using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using CarrinhoEntity = Versao1TrabalhoFinal.Models.Carrinho;
using CarrinhoItemEntity = Versao1TrabalhoFinal.Models.CarrinhoItem;
using CarrinhoServicoEntity = Versao1TrabalhoFinal.Models.CarrinhoServico;
using ClienteEntity = Versao1TrabalhoFinal.Models.Cliente;

namespace Versao1TrabalhoFinal.Pages.Carrinho
{
    /// <summary>
    /// Página responsável pela visualização do carrinho do cliente autenticado.
    /// </summary>
    [Authorize(Roles = "Cliente")]
    public class IndexModel : PageModel
    {
        private readonly StandDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Inicializa uma nova instância da página do carrinho.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        /// <param name="userManager">Gestor de utilizadores do Identity.</param>
        public IndexModel(StandDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Carrinho do cliente autenticado.
        /// </summary>
        public CarrinhoEntity? Carrinho { get; set; }

        /// <summary>
        /// Itens de veículos no carrinho.
        /// </summary>
        public List<CarrinhoItemEntity> ItensVeiculos { get; set; } = new();

        /// <summary>
        /// Itens de serviços no carrinho.
        /// </summary>
        public List<CarrinhoServicoEntity> ItensServicos { get; set; } = new();

        /// <summary>
        /// Total atual do carrinho.
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Carrega o carrinho do cliente autenticado.
        /// </summary>
        /// <returns>A página do carrinho.</returns>
        public async Task<IActionResult> OnGetAsync()
        {
            await CarregarCarrinhoAsync();
            return Page();
        }

        /// <summary>
        /// Remove um veículo do carrinho.
        /// </summary>
        /// <param name="id">Identificador do item de veículo no carrinho.</param>
        /// <returns>Redireciona para a página do carrinho.</returns>
        public async Task<IActionResult> OnPostRemoverVeiculoAsync(int id)
        {
            var cliente = await ObterClienteAutenticadoAsync();

            if (cliente == null)
            {
                return RedirectToPage("/Account/Login");
            }

            var carrinho = await _context.Carrinhos
                .FirstOrDefaultAsync(c => c.ClienteId == cliente.Id);

            if (carrinho == null)
            {
                TempData["ErrorMessage"] = "Carrinho não encontrado.";
                return RedirectToPage();
            }

            var item = await _context.CarrinhoItens
                .FirstOrDefaultAsync(ci => ci.Id == id && ci.CarrinhoId == carrinho.Id);

            if (item == null)
            {
                TempData["ErrorMessage"] = "Veículo não encontrado no carrinho.";
                return RedirectToPage();
            }

            _context.CarrinhoItens.Remove(item);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Veículo removido do carrinho com sucesso.";
            return RedirectToPage();
        }

        /// <summary>
        /// Remove um serviço do carrinho.
        /// </summary>
        /// <param name="id">Identificador do item de serviço no carrinho.</param>
        /// <returns>Redireciona para a página do carrinho.</returns>
        public async Task<IActionResult> OnPostRemoverServicoAsync(int id)
        {
            var cliente = await ObterClienteAutenticadoAsync();

            if (cliente == null)
            {
                return RedirectToPage("/Account/Login");
            }

            var carrinho = await _context.Carrinhos
                .FirstOrDefaultAsync(c => c.ClienteId == cliente.Id);

            if (carrinho == null)
            {
                TempData["ErrorMessage"] = "Carrinho não encontrado.";
                return RedirectToPage();
            }

            var itemServico = await _context.CarrinhoServicos
                .FirstOrDefaultAsync(cs => cs.Id == id && cs.CarrinhoId == carrinho.Id);

            if (itemServico == null)
            {
                TempData["ErrorMessage"] = "Serviço não encontrado no carrinho.";
                return RedirectToPage();
            }

            _context.CarrinhoServicos.Remove(itemServico);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Serviço removido do carrinho com sucesso.";
            return RedirectToPage();
        }

        /// <summary>
        /// Limpa todos os itens do carrinho.
        /// </summary>
        /// <returns>Redireciona para a página do carrinho.</returns>
        public async Task<IActionResult> OnPostLimparAsync()
        {
            var cliente = await ObterClienteAutenticadoAsync();

            if (cliente == null)
            {
                return RedirectToPage("/Account/Login");
            }

            var carrinho = await _context.Carrinhos
                .FirstOrDefaultAsync(c => c.ClienteId == cliente.Id);

            if (carrinho == null)
            {
                TempData["ErrorMessage"] = "Carrinho não encontrado.";
                return RedirectToPage();
            }

            var itensVeiculos = await _context.CarrinhoItens
                .Where(ci => ci.CarrinhoId == carrinho.Id)
                .ToListAsync();

            var itensServicos = await _context.CarrinhoServicos
                .Where(cs => cs.CarrinhoId == carrinho.Id)
                .ToListAsync();

            if (!itensVeiculos.Any() && !itensServicos.Any())
            {
                TempData["ErrorMessage"] = "O carrinho já se encontra vazio.";
                return RedirectToPage();
            }

            if (itensVeiculos.Any())
            {
                _context.CarrinhoItens.RemoveRange(itensVeiculos);
            }

            if (itensServicos.Any())
            {
                _context.CarrinhoServicos.RemoveRange(itensServicos);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Carrinho limpo com sucesso.";
            return RedirectToPage();
        }

        /// <summary>
        /// Carrega os veículos e serviços do carrinho.
        /// </summary>
        private async Task CarregarCarrinhoAsync()
        {
            var cliente = await ObterClienteAutenticadoAsync();

            if (cliente == null)
            {
                Carrinho = null;
                ItensVeiculos = new List<CarrinhoItemEntity>();
                ItensServicos = new List<CarrinhoServicoEntity>();
                Total = 0;
                return;
            }

            Carrinho = await _context.Carrinhos
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ClienteId == cliente.Id);

            if (Carrinho == null)
            {
                ItensVeiculos = new List<CarrinhoItemEntity>();
                ItensServicos = new List<CarrinhoServicoEntity>();
                Total = 0;
                return;
            }

            ItensVeiculos = await _context.CarrinhoItens
                .AsNoTracking()
                .Where(ci => ci.CarrinhoId == Carrinho.Id)
                .Include(ci => ci.VeiculoStand)
                    .ThenInclude(vs => vs.Veiculo)
                .OrderByDescending(ci => ci.DataAdicao)
                .ToListAsync();

            ItensServicos = await _context.CarrinhoServicos
                .AsNoTracking()
                .Where(cs => cs.CarrinhoId == Carrinho.Id)
                .Include(cs => cs.Servico)
                .OrderByDescending(cs => cs.DataAdicao)
                .ToListAsync();

            var totalVeiculos = ItensVeiculos.Sum(i => i.PrecoNoMomento);
            var totalServicos = ItensServicos.Sum(s => s.PrecoNoMomento);

            Total = totalVeiculos + totalServicos;
        }

        /// <summary>
        /// Obtém o cliente autenticado.
        /// </summary>
        /// <returns>Cliente autenticado ou nulo.</returns>
        private async Task<ClienteEntity?> ObterClienteAutenticadoAsync()
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
