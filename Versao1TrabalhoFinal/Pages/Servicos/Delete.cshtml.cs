using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Servicos
{
    /// <summary>
    /// Página responsável pela eliminação de serviços.
    /// </summary>
    [Authorize(Roles = "Colaborador,Admin")]
    public class DeleteModel : PageModel
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Inicializa uma nova instância da página de eliminação de serviços.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public DeleteModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Serviço a eliminar.
        /// </summary>
        [BindProperty]
        public Servico? Servico { get; set; }

        /// <summary>
        /// Carrega o serviço a eliminar.
        /// </summary>
        /// <param name="id">Identificador do serviço.</param>
        /// <returns>A página ou NotFound.</returns>
        public async Task<IActionResult> OnGetAsync(int id)
        {
            Servico = await _context.Servicos
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (Servico == null)
            {
                return NotFound();
            }

            return Page();
        }

        /// <summary>
        /// Elimina o serviço selecionado e remove também as imagens associadas da galeria.
        /// </summary>
        /// <returns>Redireciona para a listagem.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (Servico == null)
            {
                return NotFound();
            }

            var servico = await _context.Servicos
                .FirstOrDefaultAsync(s => s.Id == Servico.Id);

            if (servico == null)
            {
                return NotFound();
            }

            var imagensGaleria = await _context.ImagensEntidade
                .Where(i => i.TipoEntidade == "Servico" && i.EntidadeId == servico.Id)
                .ToListAsync();

            if (imagensGaleria.Any())
            {
                _context.ImagensEntidade.RemoveRange(imagensGaleria);
            }

            _context.Servicos.Remove(servico);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Serviço eliminado com sucesso.";
            return RedirectToPage("/Servicos/Index");
        }
    }
}