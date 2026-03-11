using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.ClienteArea
{
    [Authorize(Roles = "Cliente")]
    public class VeiculosModel : PageModel
    {
        private readonly StandDbContext _context;

        public VeiculosModel(StandDbContext context)
        {
            _context = context;
        }

        public List<Veiculo> Veiculos { get; set; } = new();

        public async Task OnGetAsync()
        {
            var email = User.Identity?.Name;

            Veiculos = await _context.Veiculos
                .Include(v => v.Cliente)
                .Where(v => v.Cliente != null && v.Cliente.Email == email)
                .ToListAsync();
        }
    }
}
