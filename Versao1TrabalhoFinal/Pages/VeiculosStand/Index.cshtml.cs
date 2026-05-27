using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.VeiculosStand
{
    public class IndexModel : PageModel
    {
        private readonly StandDbContext _context;

        public IndexModel(StandDbContext context)
        {
            _context = context;
        }

        public IList<VeiculoStand> VeiculosStand { get; set; } = new List<VeiculoStand>();

        public async Task OnGetAsync()
        {
            VeiculosStand = await _context.VeiculosStand
                .AsNoTracking()
                .Include(vs => vs.Veiculo)
                .OrderByDescending(vs => vs.DataEntrada)
                .ToListAsync();

            var ids = VeiculosStand.Select(vs => vs.Id).ToList();

            var imagens = await _context.ImagensEntidade
                .AsNoTracking()
                .Where(i => i.TipoEntidade == "VeiculoStand" && ids.Contains(i.EntidadeId))
                .OrderByDescending(i => i.Principal)
                .ThenBy(i => i.Ordem)
                .ToListAsync();

            foreach (var item in VeiculosStand)
            {
                item.Imagens = imagens
                    .Where(i => i.EntidadeId == item.Id)
                    .OrderByDescending(i => i.Principal)
                    .ThenBy(i => i.Ordem)
                    .ToList();
            }
        }
    }
}