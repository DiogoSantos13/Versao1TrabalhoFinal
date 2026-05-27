using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Veiculos
{
    [Authorize(Roles = "Cliente")]
    public class CreateModel : PageModel
    {
        private readonly StandDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CreateModel(StandDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Veiculo Veiculo { get; set; } = new();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var email = (user.Email ?? User.Identity?.Name ?? string.Empty).Trim().ToLower();

            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError(string.Empty, "Não foi possível identificar o utilizador autenticado.");
                return Page();
            }

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Email != null && c.Email.Trim().ToLower() == email);

            if (cliente == null)
            {
                ModelState.AddModelError(string.Empty, "Cliente não encontrado para o utilizador autenticado.");
                return Page();
            }

            Veiculo.ClienteId = cliente.Id;

            ModelState.Remove("Veiculo.Cliente");
            ModelState.Remove("Veiculo.Orcamentos");
            ModelState.Remove("Veiculo.OrdensReparacao");
            ModelState.Remove("Veiculo.HistoricoReparacoes");
            ModelState.Remove("Veiculo.VeiculosStand");

            if (await _context.Veiculos.AnyAsync(v => v.Matricula == Veiculo.Matricula))
            {
                ModelState.AddModelError("Veiculo.Matricula", "Já existe um veículo registado com esta matrícula.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Veiculos.Add(Veiculo);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Veículo criado com sucesso.";
            return RedirectToPage("./Index");
        }
    }
}