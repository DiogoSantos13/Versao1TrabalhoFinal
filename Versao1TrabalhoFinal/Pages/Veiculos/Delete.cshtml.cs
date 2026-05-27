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
    /// Página responsável pela eliminação de veículos.
    /// </summary>
    [Authorize(Roles = "Admin,Mecanico,Colaborador,Cliente")]
    public class DeleteModel : PageModel
    {
        private readonly StandDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DeleteModel(StandDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Veiculo Veiculo { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null)
            {
                return Challenge();
            }

            var isStaff =
                await _userManager.IsInRoleAsync(identityUser, "Admin") ||
                await _userManager.IsInRoleAsync(identityUser, "Mecanico") ||
                await _userManager.IsInRoleAsync(identityUser, "Colaborador");

            IQueryable<Veiculo> query = _context.Veiculos.AsNoTracking();

            if (!isStaff)
            {
                var email = (identityUser.Email ?? string.Empty).Trim().ToLower();

                var cliente = await _context.Clientes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Email != null && c.Email.Trim().ToLower() == email);

                if (cliente == null)
                {
                    return NotFound();
                }

                query = query.Where(v => v.ClienteId == cliente.Id);
            }

            var veiculo = await query.FirstOrDefaultAsync(v => v.Id == id);

            if (veiculo == null)
            {
                return NotFound();
            }

            veiculo.Imagens = await _context.ImagensEntidade
                .AsNoTracking()
                .Where(i => i.TipoEntidade == "Veiculo" && i.EntidadeId == id)
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

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null)
            {
                return Challenge();
            }

            var isStaff =
                await _userManager.IsInRoleAsync(identityUser, "Admin") ||
                await _userManager.IsInRoleAsync(identityUser, "Mecanico") ||
                await _userManager.IsInRoleAsync(identityUser, "Colaborador");

            IQueryable<Veiculo> query = _context.Veiculos;

            if (!isStaff)
            {
                var email = (identityUser.Email ?? string.Empty).Trim().ToLower();

                var cliente = await _context.Clientes
                    .FirstOrDefaultAsync(c => c.Email != null && c.Email.Trim().ToLower() == email);

                if (cliente == null)
                {
                    return NotFound();
                }

                query = query.Where(v => v.ClienteId == cliente.Id);
            }

            var veiculo = await query.FirstOrDefaultAsync(v => v.Id == id);

            if (veiculo == null)
            {
                return NotFound();
            }

            var imagens = await _context.ImagensEntidade
                .Where(i => i.TipoEntidade == "Veiculo" && i.EntidadeId == id)
                .ToListAsync();

            if (imagens.Any())
            {
                _context.ImagensEntidade.RemoveRange(imagens);
            }

            _context.Veiculos.Remove(veiculo);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Veículo eliminado com sucesso.";
            return RedirectToPage("./Index");
        }
    }
}