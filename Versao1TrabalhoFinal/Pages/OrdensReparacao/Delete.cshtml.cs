using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.OrdensReparacao
{
    /// <summary>
    /// Página responsável pela eliminação de ordens de reparação.
    /// </summary>
    [Authorize(Roles = "Admin,Colaborador")]
    public class DeleteModel : PageModel
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Construtor da página.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public DeleteModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Ordem a eliminar.
        /// </summary>
        [BindProperty]
        public OrdemReparacao OrdemReparacao { get; set; } = new();

        /// <summary>
        /// Carrega a ordem antes da confirmação.
        /// </summary>
        /// <param name="id">Id da ordem.</param>
        /// <returns>Página ou NotFound.</returns>
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var ordem = await _context.OrdensReparacao
                .Include(o => o.Cliente)
                .Include(o => o.Veiculo)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (ordem == null)
                return NotFound();

            OrdemReparacao = ordem;
            return Page();
        }

        /// <summary>
        /// Elimina a ordem selecionada.
        /// </summary>
        /// <param name="id">Id da ordem.</param>
        /// <returns>Redireciona para a listagem.</returns>
        public async Task<IActionResult> OnPostAsync(int id)
        {
            var ordem = await _context.OrdensReparacao.FindAsync(id);

            if (ordem != null)
            {
                _context.OrdensReparacao.Remove(ordem);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
