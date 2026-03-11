using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Servicos
{
    [Authorize(Roles = "Colaborador,Admin")]
    public class IndexModel : PageModel
    {
        private readonly StandDbContext _context;

        public IndexModel(StandDbContext context)
        {
            _context = context;
        }

        public List<Servico> Servicos { get; set; } = new();

        public async Task OnGetAsync()
        {
            Servicos = await _context.Servicos
                .OrderBy(s => s.Nome)
                .ToListAsync();
        }
    }
}
