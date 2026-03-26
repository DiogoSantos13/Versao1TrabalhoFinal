using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Servicos
{
    /// <summary>
    /// Página responsável pelos detalhes de um serviço.
    /// </summary>
    [Authorize(Roles = "Cliente,Colaborador,Admin")]

    public class DetailsModel : PageModel
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Inicializa uma nova instância da página de detalhes de serviço.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public DetailsModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Serviço carregado.
        /// </summary>
        public Servico? Servico { get; set; }

        /// <summary>
        /// Carrega os detalhes do serviço.
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
    }
}
