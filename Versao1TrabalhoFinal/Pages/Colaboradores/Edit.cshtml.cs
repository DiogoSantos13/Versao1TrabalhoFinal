using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Colaboradores
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly StandDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EditModel(
            StandDbContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public List<SelectListItem> RolesDisponiveis { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            await CarregarRolesAsync();

            var colaborador = await _context.Colaboradores
                .FirstOrDefaultAsync(c => c.Id == id);

            if (colaborador == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(colaborador.IdentityUserId);
            if (user == null)
            {
                return NotFound();
            }

            var rolesAtuais = await _userManager.GetRolesAsync(user);

            Input = new InputModel
            {
                Id = colaborador.Id,
                IdentityUserId = colaborador.IdentityUserId,
                Nome = colaborador.Nome,
                Telefone = colaborador.Telefone,
                Email = colaborador.Email,
                NIF = colaborador.NIF,
                Morada = colaborador.Morada,
                ImagemUrl = colaborador.ImagemUrl,
                Cargo = colaborador.Cargo,
                Role = ObterRolePrincipal(rolesAtuais)
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await CarregarRolesAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var colaborador = await _context.Colaboradores
                .FirstOrDefaultAsync(c => c.Id == Input.Id);

            if (colaborador == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(colaborador.IdentityUserId);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Não foi possível encontrar o utilizador associado.");
                return Page();
            }

            var emailEmUsoIdentity = await _userManager.FindByEmailAsync(Input.Email);
            if (emailEmUsoIdentity != null && emailEmUsoIdentity.Id != user.Id)
            {
                ModelState.AddModelError("Input.Email", "Já existe outro utilizador com este email.");
                return Page();
            }

            var emailEmUsoColaborador = await _context.Colaboradores
                .AnyAsync(c => c.Email == Input.Email && c.Id != Input.Id);

            if (emailEmUsoColaborador)
            {
                ModelState.AddModelError("Input.Email", "Já existe outro colaborador com este email.");
                return Page();
            }

            colaborador.Nome = Input.Nome;
            colaborador.Telefone = Input.Telefone;
            colaborador.Email = Input.Email;
            colaborador.NIF = Input.NIF;
            colaborador.Morada = Input.Morada;
            colaborador.ImagemUrl = Input.ImagemUrl;
            colaborador.Cargo = Input.Cargo;

            user.UserName = Input.Email;
            user.Email = Input.Email;
            user.PhoneNumber = Input.Telefone;

            var updateUserResult = await _userManager.UpdateAsync(user);
            if (!updateUserResult.Succeeded)
            {
                foreach (var error in updateUserResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return Page();
            }

            var rolesAtuais = await _userManager.GetRolesAsync(user);
            var rolesParaRemover = rolesAtuais
                .Where(r => r != "Colaborador")
                .ToList();

            if (rolesParaRemover.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesParaRemover);
                if (!removeResult.Succeeded)
                {
                    foreach (var error in removeResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return Page();
                }
            }

            if (!string.IsNullOrWhiteSpace(Input.Role) &&
                !string.Equals(Input.Role, "Colaborador", StringComparison.OrdinalIgnoreCase) &&
                !await _userManager.IsInRoleAsync(user, Input.Role))
            {
                var addRoleResult = await _userManager.AddToRoleAsync(user, Input.Role);
                if (!addRoleResult.Succeeded)
                {
                    foreach (var error in addRoleResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return Page();
                }
            }

            if (!await _userManager.IsInRoleAsync(user, "Colaborador"))
            {
                var addColaboradorRole = await _userManager.AddToRoleAsync(user, "Colaborador");
                if (!addColaboradorRole.Succeeded)
                {
                    foreach (var error in addColaboradorRole.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return Page();
                }
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Colaborador atualizado com sucesso.";
            return RedirectToPage("./Index");
        }

        private async Task CarregarRolesAsync()
        {
            var rolesPermitidas = new[]
            {
                "Admin",
                "Mecanico",
                "Rececionista",
                "Vendedor",
                "Funcionario",
                "Colaborador"
            };

            RolesDisponiveis = await _roleManager.Roles
                .Where(r => r.Name != null && rolesPermitidas.Contains(r.Name))
                .OrderBy(r => r.Name)
                .Select(r => new SelectListItem
                {
                    Value = r.Name!,
                    Text = r.Name!
                })
                .ToListAsync();
        }

        private static string ObterRolePrincipal(IList<string> roles)
        {
            return roles.FirstOrDefault(r => r != "Colaborador") ?? "Colaborador";
        }

        public class InputModel
        {
            [Required]
            public int Id { get; set; }

            [Required]
            public string IdentityUserId { get; set; } = string.Empty;

            [Required(ErrorMessage = "O nome é obrigatório.")]
            [StringLength(100)]
            public string Nome { get; set; } = string.Empty;

            [Phone]
            [StringLength(20)]
            public string? Telefone { get; set; }

            [Required(ErrorMessage = "O email é obrigatório.")]
            [EmailAddress]
            [StringLength(150)]
            public string Email { get; set; } = string.Empty;

            [StringLength(20)]
            public string? NIF { get; set; }

            [StringLength(250)]
            public string? Morada { get; set; }

            [StringLength(500)]
            public string? ImagemUrl { get; set; }

            [StringLength(100)]
            public string? Cargo { get; set; }

            [Required(ErrorMessage = "A role é obrigatória.")]
            public string Role { get; set; } = "Colaborador";
        }
    }
}