using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;

namespace Versao1TrabalhoFinal.Pages.Carrinho
{
    /// <summary>
    /// Página responsável pela remoção de itens do carrinho.
    /// </summary>
    [Authorize(Roles = "Cliente")]
    public class RemoverModel : PageModel
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Inicializa uma nova instância da página de remoção de itens.
        /// </summary>
        public RemoverModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Carrega a página de remoção.
        /// </summary>
        public void OnGet()
        {
        }

        /// <summary>
        /// Remove um serviço do carrinho.
        /// </summary>
        /// <param name="id">Identificador do item de serviço no carrinho.</param>
        public async Task<IActionResult> OnGetServicoAsync(int id)
        {
            var item = await _context.CarrinhoServicos
                .FirstOrDefaultAsync(cs => cs.Id == id);

            if (item == null)
            {
                TempData["ErrorMessage"] = "O serviço do carrinho não foi encontrado.";
                return RedirectToPage("/Carrinho/Index");
            }

            _context.CarrinhoServicos.Remove(item);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Serviço removido do carrinho com sucesso.";
            return RedirectToPage("/Carrinho/Index");
        }

        /// <summary>
        /// Remove um produto do carrinho.
        /// </summary>
        /// <param name="id">Identificador do item de produto no carrinho.</param>
        public async Task<IActionResult> OnGetProdutoAsync(int id)
        {
            var item = await _context.CarrinhoProdutos
                .FirstOrDefaultAsync(cp => cp.Id == id);

            if (item == null)
            {
                TempData["ErrorMessage"] = "O produto do carrinho não foi encontrado.";
                return RedirectToPage("/Carrinho/Index");
            }

            _context.CarrinhoProdutos.Remove(item);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Produto removido do carrinho com sucesso.";
            return RedirectToPage("/Carrinho/Index");
        }
    }
}