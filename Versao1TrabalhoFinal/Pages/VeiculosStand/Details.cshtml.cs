using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.VeiculosStand
{
    /// <summary>
    /// Página pública de detalhes de um veículo do stand.
    /// </summary>
    public class DetailsModel : PageModel
    {
        private readonly StandDbContext _context;

        public DetailsModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Veículo do stand a apresentar.
        /// </summary>
        public VeiculoStand VeiculoStand { get; set; } = new();

        /// <summary>
        /// Carrega os detalhes do veículo do stand, do veículo associado e respetiva galeria.
        /// </summary>
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var item = await _context.VeiculosStand
                .AsNoTracking()
                .Include(vs => vs.Veiculo)
                .FirstOrDefaultAsync(vs => vs.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            item.Imagens = await _context.ImagensEntidade
                .AsNoTracking()
                .Where(i => i.TipoEntidade == "VeiculoStand" && i.EntidadeId == item.Id)
                .OrderByDescending(i => i.Principal)
                .ThenBy(i => i.Ordem)
                .ToListAsync();

            if (!item.Imagens.Any() && !string.IsNullOrWhiteSpace(item.Veiculo?.ImagemUrl))
            {
                item.Imagens.Add(new ImagemEntidade
                {
                    Url = item.Veiculo.ImagemUrl,
                    Alt = $"{item.Veiculo?.Marca} {item.Veiculo?.Modelo}",
                    Principal = true,
                    Ordem = 0
                });
            }

            if (!item.Imagens.Any())
            {
                item.Imagens.Add(new ImagemEntidade
                {
                    Url = "/images/cars/default-car.jpg",
                    Alt = "Imagem por defeito",
                    Principal = true,
                    Ordem = 0
                });
            }

            VeiculoStand = item;
            return Page();
        }
    }
}