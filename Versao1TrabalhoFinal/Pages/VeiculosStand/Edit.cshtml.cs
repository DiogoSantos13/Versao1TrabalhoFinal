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
    public class EditModel : PageModel
    {
        private readonly StandDbContext _context;

        public EditModel(StandDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public VeiculoStand VeiculoStand { get; set; } = default!;

        public SelectList VeiculosSelect { get; set; } = default!;

        private async Task LoadVeiculosSelectAsync(object? selectedValue = null)
        {
            var veiculos = await _context.Veiculos
                .AsNoTracking()
                .OrderBy(v => v.Marca)
                .ThenBy(v => v.Modelo)
                .Select(v => new
                {
                    v.Id,
                    NomeCompleto = (v.Marca ?? "") + " " + (v.Modelo ?? "")
                })
                .ToListAsync();

            VeiculosSelect = new SelectList(veiculos, "Id", "NomeCompleto", selectedValue);
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var item = await _context.VeiculosStand
                .AsNoTracking()
                .FirstOrDefaultAsync(vs => vs.Id == id);

            if (item == null)
                return NotFound();

            VeiculoStand = item;

            await LoadVeiculosSelectAsync(VeiculoStand.VeiculoId);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                await LoadVeiculosSelectAsync(VeiculoStand.VeiculoId);
                return Page();
            }

            var entityToUpdate = await _context.VeiculosStand
                .FirstOrDefaultAsync(vs => vs.Id == id);

            if (entityToUpdate == null)
                return NotFound();

            var updated = await TryUpdateModelAsync(
                entityToUpdate,
                "VeiculoStand",
                v => v.VeiculoId,
                v => v.Preco,
                v => v.Estado,
                v => v.Quilometros,
                v => v.Descricao,
                v => v.DataEntrada,
                v => v.Cilindrada,
                v => v.ImagemUrl
            );

            if (!updated)
            {
                await LoadVeiculosSelectAsync(entityToUpdate.VeiculoId);
                return Page();
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}