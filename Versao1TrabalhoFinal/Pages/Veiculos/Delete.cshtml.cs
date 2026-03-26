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

        /// <summary>
        /// Inicializa uma nova instância da página de eliminação de veículos.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        /// <param name="userManager">Gestor de utilizadores do Identity.</param>
        public DeleteModel(StandDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Veículo a apresentar na confirmação de eliminação.
        /// </summary>
        [BindProperty]
        public Veiculo Veiculo { get; set; } = new();

        /// <summary>
        /// Carrega a página de confirmação de eliminação do veículo.
        /// </summary>
        /// <param name="id">Identificador do veículo.</param>
        /// <returns>A página de confirmação ou um resultado adequado caso não exista acesso.</returns>
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

            Veiculo = veiculo;
            return Page();
        }

        /// <summary>
        /// Remove o veículo selecionado, respeitando as permissões do utilizador autenticado.
        /// </summary>
        /// <param name="id">Identificador do veículo.</param>
        /// <returns>Redireciona para a listagem após a eliminação.</returns>
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

            _context.Veiculos.Remove(veiculo);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Veículo eliminado com sucesso.";
            return RedirectToPage("./Index");
        }
    }
}
