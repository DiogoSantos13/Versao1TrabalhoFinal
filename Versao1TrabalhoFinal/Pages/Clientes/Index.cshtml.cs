using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Clientes
{
    /// <summary>
    /// Página responsável pela listagem de clientes.
    /// </summary>
    [Authorize(Roles = "Admin,Colaborador")]
    public class IndexModel : PageModel
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Inicializa uma nova instância da página de listagem de clientes.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public IndexModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista de clientes.
        /// </summary>
        public List<Cliente> Clientes { get; set; } = new();

        /// <summary>
        /// Carrega a lista de clientes.
        /// </summary>
        /// <returns>Tarefa assíncrona.</returns>
        public async Task OnGetAsync()
        {
            Clientes = await _context.Clientes
                .OrderBy(c => c.Nome)
                .ToListAsync();
        }
    }
}
