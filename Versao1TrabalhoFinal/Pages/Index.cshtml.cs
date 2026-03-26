using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages
{
    public class IndexModel : PageModel
    {
        private readonly StandDbContext _context;

        public IndexModel(StandDbContext context)
        {
            _context = context;
        }

        public List<VeiculoStand> VeiculosDestaque { get; set; } = new();

        public async Task OnGetAsync()
        {
            VeiculosDestaque = await _context.VeiculosStand
                .AsNoTracking()
                .OrderByDescending(v => v.DataEntrada)
                .Take(6)
                .ToListAsync();
        }
    }
}
