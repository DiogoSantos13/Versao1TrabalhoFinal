using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using ClienteEntity = Versao1TrabalhoFinal.Models.Cliente;

namespace Versao1TrabalhoFinal.Pages.Clientes
{
    /// <summary>
    /// Página responsável pela listagem de clientes.
    /// </summary>
    [Authorize(Roles = "Admin,Colaborador")]
    public class IndexModel : PageModel
    {
        /// <summary>
        /// Contexto da base de dados da aplicação.
        /// </summary>
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
        public List<ClienteEntity> Clientes { get; set; } = new();

        /// <summary>
        /// Carrega a lista de clientes.
        /// </summary>
        /// <returns>Tarefa assíncrona.</returns>
        public async Task OnGetAsync()
        {
            // Obtém a lista de clientes ordenada por nome.
            Clientes = await _context.Clientes
                .OrderBy(c => c.Nome)
                .ToListAsync();
        }
    }
}