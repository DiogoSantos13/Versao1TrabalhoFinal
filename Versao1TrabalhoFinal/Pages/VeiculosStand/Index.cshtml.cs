using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.VeiculosStand
{
    /// <summary>
    /// Página de listagem de veículos do stand.
    /// </summary>
    [Authorize(Roles = "Admin,Vendedor")]
    public class IndexModel : PageModel
    {
        private readonly StandDbContext _context;

        public IndexModel(StandDbContext context)
        {
            _context = context;
        }

        public List<VeiculoStand> VeiculosStand { get; set; } = new();

        public async Task OnGetAsync()
        {
            VeiculosStand = await _context.VeiculosStand
                .Include(v => v.Veiculo)
                .OrderByDescending(v => v.DataEntrada)
                .ToListAsync();
        }
    }
}
