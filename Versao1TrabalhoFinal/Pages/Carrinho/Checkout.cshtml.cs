using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using CarrinhoItemEntity = Versao1TrabalhoFinal.Models.CarrinhoItem;
using ClienteEntity = Versao1TrabalhoFinal.Models.Cliente;
using EstadoVeiculoStandEntity = Versao1TrabalhoFinal.Models.EstadoVeiculoStand;

namespace Versao1TrabalhoFinal.Pages.Carrinho
{
    /// <summary>
    /// Página responsável pela confirmação do checkout do carrinho.
    /// </summary>
    [Authorize(Roles = "Cliente")]
    public class CheckoutModel : PageModel
    {
        private readonly StandDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Inicializa uma nova instância da página de checkout.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        /// <param name="userManager">Gestor de utilizadores do Identity.</param>
        public CheckoutModel(StandDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Total do carrinho.
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Lista de itens do carrinho.
        /// </summary>
        public List<CarrinhoItemEntity> Itens { get; set; } = new();

        /// <summary>
        /// Carrega os dados do carrinho para confirmação.
        /// </summary>
        /// <returns>A página de checkout.</returns>
        public async Task<IActionResult> OnGetAsync()
        {
            var cliente = await ObterClienteAutenticadoAsync();

            if (cliente == null)
            {
                return RedirectToPage("/Account/Login");
            }

            var carrinho = await _context.Carrinhos
                .AsNoTracking()
                .Include(c => c.Itens)
                    .ThenInclude(i => i.VeiculoStand)
                        .ThenInclude(vs => vs.Veiculo)
                .FirstOrDefaultAsync(c => c.ClienteId == cliente.Id);

            if (carrinho == null || !carrinho.Itens.Any())
            {
                TempData["ErrorMessage"] = "O carrinho está vazio.";
                return RedirectToPage("/Carrinho/Index");
            }

            Itens = carrinho.Itens.ToList();
            Total = Itens.Sum(i => i.PrecoNoMomento);

            return Page();
        }

        /// <summary>
        /// Confirma o pedido e marca os veículos como vendidos.
        /// </summary>
        /// <returns>Redireciona para a página do carrinho.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            var cliente = await ObterClienteAutenticadoAsync();

            if (cliente == null)
            {
                return RedirectToPage("/Account/Login");
            }

            var carrinho = await _context.Carrinhos
                .Include(c => c.Itens)
                    .ThenInclude(i => i.VeiculoStand)
                .FirstOrDefaultAsync(c => c.ClienteId == cliente.Id);

            if (carrinho == null || !carrinho.Itens.Any())
            {
                TempData["ErrorMessage"] = "O carrinho está vazio.";
                return RedirectToPage("/Carrinho/Index");
            }

            foreach (var item in carrinho.Itens)
            {
                if (!string.Equals(item.VeiculoStand.Estado, EstadoVeiculoStandEntity.Disponivel, StringComparison.OrdinalIgnoreCase))
                {
                    TempData["ErrorMessage"] = "Um dos veículos já não se encontra disponível.";
                    return RedirectToPage("/Carrinho/Index");
                }
            }

            foreach (var item in carrinho.Itens)
            {
                item.VeiculoStand.Estado = EstadoVeiculoStandEntity.Vendido;
            }

            _context.CarrinhoItens.RemoveRange(carrinho.Itens);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Pedido confirmado com sucesso.";
            return RedirectToPage("/Carrinho/Index");
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
                .FirstOrDefaultAsync(c => c.IdentityUserId == userId);
        }
    }
}
