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

        /// <summary>
        /// Inicializa uma nova instância da página de detalhes de veículo.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        /// <param name="userManager">Gestor de utilizadores do Identity.</param>
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

            Veiculo = veiculo;
            return Page();
        }
    }
}
