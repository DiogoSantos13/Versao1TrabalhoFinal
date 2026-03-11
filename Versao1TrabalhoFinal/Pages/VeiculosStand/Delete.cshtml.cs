using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.VeiculosStand
{
    /// <summary>
    /// Página de eliminação de veículos do stand.
    /// </summary>
    [Authorize(Roles = "Admin,Vendedor")]
    public class DeleteModel : PageModel
    {
        private readonly StandDbContext _context;

        public DeleteModel(StandDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public VeiculoStand VeiculoStand { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var item = await _context.VeiculosStand
                .Include(v => v.Veiculo)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (item == null)
                return NotFound();

            VeiculoStand = item;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var item = await _context.VeiculosStand.FindAsync(id);

            if (item != null)
            {
                _context.VeiculosStand.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
