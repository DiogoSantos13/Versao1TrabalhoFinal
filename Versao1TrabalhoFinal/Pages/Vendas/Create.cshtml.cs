using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Vendas
{
    /// <summary>
    /// Página de criação de vendas.
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
        public Venda Venda { get; set; } = new();

        public SelectList ClientesSelect { get; set; } = default!;

        public void OnGet()
        {
            Venda.DataVenda = DateTime.Now;
            ClientesSelect = new SelectList(_context.Clientes.OrderBy(c => c.Nome).ToList(), "Id", "Nome");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ClientesSelect = new SelectList(_context.Clientes.OrderBy(c => c.Nome).ToList(), "Id", "Nome");
                return Page();
            }

            _context.Vendas.Add(Venda);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
