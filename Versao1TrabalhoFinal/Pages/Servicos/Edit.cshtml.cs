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
    [Authorize(Roles = "Colaborador,Admin")]
    public class EditModel : PageModel
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Inicializa uma nova instância da página de edição de serviços.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public EditModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Serviço a editar.
        /// </summary>
        [BindProperty]
        public Servico Servico { get; set; } = new();

        /// <summary>
        /// Carrega o serviço a editar.
        /// </summary>
        /// <param name="id">Identificador do serviço.</param>
        /// <returns>A página ou NotFound.</returns>
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var servico = await _context.Servicos.FirstOrDefaultAsync(s => s.Id == id);

            if (servico == null)
            {
                return NotFound();
            }

            Servico = servico;
            return Page();
        }

        /// <summary>
        /// Guarda as alterações do serviço.
        /// </summary>
        /// <returns>Redireciona para a listagem ou volta à página em caso de erro.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Servico).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var existe = await _context.Servicos.AnyAsync(s => s.Id == Servico.Id);

                if (!existe)
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToPage("/Servicos/Index");
        }
    }
}
