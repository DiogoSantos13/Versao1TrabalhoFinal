using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Servicos
{
    /// <summary>
    /// Página responsável pela edição de serviços.
    /// </summary>
    [Authorize(Roles = "Admin,Mecanico")]
    public class EditModel : PageModel
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Construtor da página.
        /// </summary>
        public EditModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Serviço ligado ao formulário.
        /// </summary>
        [BindProperty]
        public Servico Servico { get; set; } = new();

        /// <summary>
        /// Carrega o serviço para edição.
        /// </summary>
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var servico = await _context.Servicos.FindAsync(id);

            if (servico == null)
                return NotFound();

            Servico = servico;
            return Page();
        }

        /// <summary>
        /// Guarda as alterações do serviço.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.Attach(Servico).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
