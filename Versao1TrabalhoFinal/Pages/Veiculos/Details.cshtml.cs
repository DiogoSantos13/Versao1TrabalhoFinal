using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Veiculos
{
    /// <summary>
    /// Página responsável pela apresentação dos detalhes de um veículo do cliente.
    /// </summary>
    [Authorize(Roles = "Cliente")]
    public class DetailsModel : PageModel
    {
        private readonly StandDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DetailsModel(StandDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Veículo a apresentar na página de detalhes.
        /// </summary>
        public Veiculo Veiculo { get; set; } = default!;

        /// <summary>
        /// Carrega os detalhes do veículo do cliente autenticado.
        /// </summary>
        /// <param name="id">Identificador do veículo.</param>
        /// <returns>A página de detalhes ou um resultado adequado caso o veículo não exista.</returns>
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null)
            {
                return Challenge();
            }

            var email = (identityUser.Email ?? string.Empty).Trim().ToLower();

            var cliente = await _context.Clientes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Email != null && c.Email.Trim().ToLower() == email);

            if (cliente == null)
            {
                return NotFound();
            }

            var veiculo = await _context.Veiculos
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == id && v.ClienteId == cliente.Id);

            if (veiculo == null)
            {
                return NotFound();
            }

            veiculo.Imagens = await _context.ImagensEntidade
                .AsNoTracking()
                .Where(i => i.TipoEntidade == "Veiculo" && i.EntidadeId == veiculo.Id)
                .OrderByDescending(i => i.Principal)
                .ThenBy(i => i.Ordem)
                .ToListAsync();

            if (!veiculo.Imagens.Any() && !string.IsNullOrWhiteSpace(veiculo.ImagemUrl))
            {
                veiculo.Imagens.Add(new ImagemEntidade
                {
                    Url = veiculo.ImagemUrl,
                    Alt = $"{veiculo.Marca} {veiculo.Modelo}",
                    Principal = true,
                    Ordem = 0
                });
            }

            if (!veiculo.Imagens.Any())
            {
                veiculo.Imagens.Add(new ImagemEntidade
                {
                    Url = "/images/cars/default-car.jpg",
                    Alt = "Imagem por defeito",
                    Principal = true,
                    Ordem = 0
                });
            }

            Veiculo = veiculo;
            return Page();
        }
    }
}