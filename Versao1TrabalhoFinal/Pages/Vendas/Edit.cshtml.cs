using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Vendas
{
    /// <summary>
    /// Página de edição de vendas.
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
        public Venda Venda { get; set; } = new();

        public SelectList ClientesSelect { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var venda = await _context.Vendas.FindAsync(id);

            if (venda == null)
                return NotFound();

            Venda = venda;
            ClientesSelect = new SelectList(await _context.Clientes.OrderBy(c => c.Nome).ToListAsync(), "Id", "Nome", Venda.ClienteId);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ClientesSelect = new SelectList(await _context.Clientes.OrderBy(c => c.Nome).ToListAsync(), "Id", "Nome", Venda.ClienteId);
                return Page();
            }

            _context.Attach(Venda).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
