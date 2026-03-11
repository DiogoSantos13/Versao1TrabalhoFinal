using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.VeiculosStand
{
    /// <summary>
    /// Página de edição de veículos do stand.
    /// </summary>
    [Authorize(Roles = "Admin,Vendedor")]
    public class EditModel : PageModel
    {
        private readonly StandDbContext _context;

        public EditModel(StandDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public VeiculoStand VeiculoStand { get; set; } = new();

        public SelectList VeiculosSelect { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var item = await _context.VeiculosStand.FindAsync(id);

            if (item == null)
                return NotFound();

            VeiculoStand = item;
            VeiculosSelect = new SelectList(await _context.Veiculos.OrderBy(v => v.Marca).ThenBy(v => v.Modelo).ToListAsync(), "Id", "Modelo", VeiculoStand.VeiculoId);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                VeiculosSelect = new SelectList(await _context.Veiculos.OrderBy(v => v.Marca).ThenBy(v => v.Modelo).ToListAsync(), "Id", "Modelo", VeiculoStand.VeiculoId);
                return Page();
            }

            _context.Attach(VeiculoStand).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
