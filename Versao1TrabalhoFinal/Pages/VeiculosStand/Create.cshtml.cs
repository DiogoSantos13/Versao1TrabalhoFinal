using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.VeiculosStand
{
    [Authorize(Roles = "Admin")]
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

        public async Task OnGetAsync()
        {
            VeiculoStand.DataEntrada = DateTime.Now;
            await LoadVeiculosAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var veiculoExiste = await _context.Veiculos
                .AsNoTracking()
                .AnyAsync(v => v.Id == VeiculoStand.VeiculoId);

            if (!veiculoExiste)
            {
                ModelState.AddModelError("VeiculoStand.VeiculoId", "Selecione um veículo válido.");
            }

            if (!ModelState.IsValid)
            {
                await LoadVeiculosAsync();
                return Page();
            }

            _context.VeiculosStand.Add(VeiculoStand);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private async Task LoadVeiculosAsync()
        {
            var veiculos = await _context.Veiculos
                .AsNoTracking()
                .OrderBy(v => v.Marca)
                .ThenBy(v => v.Modelo)
                .Select(v => new
                {
                    v.Id,
                    NomeCompleto = v.Marca + " " + v.Modelo
                })
                .ToListAsync();

            VeiculosSelect = new SelectList(veiculos, "Id", "NomeCompleto");
        }
    }
}
