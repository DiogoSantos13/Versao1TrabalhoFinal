using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.VeiculosStand
{
    /// <summary>
    /// Página de criação de veículos do stand.
    /// </summary>
    [Authorize(Roles = "Admin,Vendedor")]
    public class CreateModel : PageModel
    {
        private readonly StandDbContext _context;

        public CreateModel(StandDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public VeiculoStand VeiculoStand { get; set; } = new();

        public SelectList VeiculosSelect { get; set; } = default!;

        public void OnGet()
        {
            VeiculoStand.DataEntrada = DateTime.Now;
            VeiculosSelect = new SelectList(_context.Veiculos.OrderBy(v => v.Marca).ThenBy(v => v.Modelo).ToList(), "Id", "Modelo");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                VeiculosSelect = new SelectList(_context.Veiculos.OrderBy(v => v.Marca).ThenBy(v => v.Modelo).ToList(), "Id", "Modelo");
                return Page();
            }

            _context.VeiculosStand.Add(VeiculoStand);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
