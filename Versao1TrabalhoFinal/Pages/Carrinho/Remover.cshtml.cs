using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;

namespace Versao1TrabalhoFinal.Pages.Carrinho
{
    /// <summary>
    /// Página responsável por remover um item do carrinho do cliente autenticado.
    /// </summary>
    [Authorize(Roles = "Cliente")]
    public class RemoverModel : PageModel
    {
        private readonly StandDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Inicializa uma nova instância da página de remoção de itens do carrinho.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        /// <param name="userManager">Gestor de utilizadores do Identity.</param>
        public RemoverModel(StandDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Remove um item do carrinho do cliente autenticado.
        /// </summary>
        /// <param name="id">Identificador do item do carrinho.</param>
        /// <returns>Redireciona para a página do carrinho.</returns>
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return RedirectToPage("/Account/Login");
            }

            var cliente = await _context.Clientes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.IdentityUserId == userId);

            if (cliente == null)
            {
                return RedirectToPage("/Account/Login");
            }

            var carrinho = await _context.Carrinhos
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ClienteId == cliente.Id);

            if (carrinho == null)
            {
                TempData["ErrorMessage"] = "Carrinho não encontrado.";
                return RedirectToPage("/Carrinho/Index");
            }

            var item = await _context.CarrinhoItens
                .FirstOrDefaultAsync(ci => ci.Id == id && ci.CarrinhoId == carrinho.Id);

            if (item == null)
            {
                TempData["ErrorMessage"] = "Item não encontrado no carrinho.";
                return RedirectToPage("/Carrinho/Index");
            }

            _context.CarrinhoItens.Remove(item);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Veículo removido do carrinho com sucesso.";
            return RedirectToPage("/Carrinho/Index");
        }
    }
}
