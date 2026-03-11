using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Veiculos
{
    [Authorize(Roles = "Admin,Colaborador")]
    public class IndexModel : PageModel
    {
        private readonly StandDbContext _context;

        public IndexModel(StandDbContext context)
        {
            _context = context;
        }

        public List<Veiculo> Veiculos { get; set; } = new();

        public async Task OnGetAsync()
        {
            Veiculos = await _context.Veiculos
                .Include(v => v.Cliente)
                .OrderBy(v => v.Marca)
                .ThenBy(v => v.Modelo)
                .ToListAsync();
        }
    }
}
