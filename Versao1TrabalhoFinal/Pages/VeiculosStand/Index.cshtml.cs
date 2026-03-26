using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.VeiculosStand
{
    /// <summary>
    /// Página responsável pela listagem dos veículos disponíveis no stand.
    /// </summary>
    public class IndexModel : PageModel
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Inicializa uma nova instância da página de listagem de veículos do stand.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public IndexModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista de veículos do stand a apresentar na página.
        /// </summary>
        public IList<VeiculoStand> VeiculosStand { get; set; } = new List<VeiculoStand>();

        /// <summary>
        /// Carrega os veículos do stand com os respetivos dados do veículo associado.
        /// </summary>
        public async Task OnGetAsync()
        {
            VeiculosStand = await _context.VeiculosStand
                .AsNoTracking()
                .Include(vs => vs.Veiculo)
                .OrderByDescending(vs => vs.DataEntrada)
                .ToListAsync();
        }
    }
}
