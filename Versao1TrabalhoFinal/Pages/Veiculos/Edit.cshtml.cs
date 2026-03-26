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
    /// Página responsável pela edição de veículos.
    /// </summary>
    [Authorize(Roles = "Admin,Mecanico,Colaborador,Cliente")]
    public class EditModel : PageModel
    {
        private readonly StandDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Inicializa uma nova instância da página de edição de veículos.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        /// <param name="userManager">Gestor de utilizadores do Identity.</param>
        public EditModel(StandDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Veículo utilizado no formulário de edição.
        /// </summary>
        [BindProperty]
        public Veiculo Veiculo { get; set; } = new();

        /// <summary>
        /// Carrega os dados do veículo a editar.
        /// </summary>
        /// <param name="id">Identificador do veículo.</param>
        /// <returns>A página de edição ou um resultado adequado caso o veículo não exista.</returns>
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
        /// Guarda as alterações efetuadas ao veículo.
        /// </summary>
        /// <returns>Redireciona para a listagem após guardar as alterações.</returns>
        public async Task<IActionResult> OnPostAsync()
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

            ModelState.Remove("Veiculo.Cliente");
            ModelState.Remove("Veiculo.Orcamentos");
            ModelState.Remove("Veiculo.OrdensReparacao");
            ModelState.Remove("Veiculo.HistoricoReparacoes");
            ModelState.Remove("Veiculo.VeiculosStand");

            if (!ModelState.IsValid)
            {
                return Page();
            }

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

            var veiculoDb = await query.FirstOrDefaultAsync(v => v.Id == Veiculo.Id);

            if (veiculoDb == null)
            {
                return NotFound();
            }

            veiculoDb.Marca = Veiculo.Marca;
            veiculoDb.Modelo = Veiculo.Modelo;
            veiculoDb.Ano = Veiculo.Ano;
            veiculoDb.Cilindrada = Veiculo.Cilindrada;
            veiculoDb.Combustivel = Veiculo.Combustivel;
            veiculoDb.Matricula = Veiculo.Matricula;
            veiculoDb.VIN = Veiculo.VIN;
            veiculoDb.ImagemUrl = Veiculo.ImagemUrl;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Veículo atualizado com sucesso.";
            return RedirectToPage("./Index");
        }
    }
}
