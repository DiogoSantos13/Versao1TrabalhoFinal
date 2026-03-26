using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.ClienteArea
{
    [Authorize(Roles = "Cliente")]
    public class HistoricoModel : PageModel
    {
        private readonly StandDbContext _context;

        public HistoricoModel(StandDbContext context)
        {
            _context = context;
        }

        public List<HistoricoReparacao> Historico { get; set; } = new();

        public async Task OnGetAsync()
        {
            var email = User.Identity?.Name;

            if (string.IsNullOrWhiteSpace(email))
            {
                Historico = new List<HistoricoReparacao>();
                return;
            }

            Historico = await _context.HistoricoReparacoes
                .Include(h => h.Veiculo)
                .ThenInclude(v => v.ClienteId)
                .Where(h => h.Veiculo != null &&
                            h.Veiculo.Cliente != null &&
                            h.Veiculo.Cliente.Email == email)
                .OrderByDescending(h => h.DataReparacao)
                .ToListAsync();
        }
    }
}
