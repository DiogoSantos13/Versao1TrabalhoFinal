using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Veiculos
{
    [Authorize(Roles = "Admin,Colaborador")]
    public class CreateModel : PageModel
    {
        private readonly StandDbContext _context;

        public CreateModel(StandDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Veiculo Veiculo { get; set; } = new();

        public SelectList ClientesSelect { get; set; } = default!;

        public void OnGet()
        {
            ClientesSelect = new SelectList(_context.Clientes.OrderBy(c => c.Nome).ToList(), "Id", "Nome");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ClientesSelect = new SelectList(_context.Clientes.OrderBy(c => c.Nome).ToList(), "Id", "Nome");
                return Page();
            }

            _context.Veiculos.Add(Veiculo);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
