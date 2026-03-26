using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Servicos
{
    /// <summary>
    /// Página responsável pela listagem de serviços.
    /// </summary>
    [Authorize(Roles = "Cliente,Colaborador,Admin")]

    public class IndexModel : PageModel
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Inicializa uma nova instância da página de listagem de serviços.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public IndexModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista de serviços.
        /// </summary>
        public List<Servico> Servicos { get; set; } = new();

        /// <summary>
        /// Carrega a lista de serviços.
        /// </summary>
        /// <returns>Tarefa assíncrona.</returns>
        public async Task OnGetAsync()
        {
            Servicos = await _context.Servicos
                .OrderBy(s => s.Nome)
                .ToListAsync();
        }
    }
}
