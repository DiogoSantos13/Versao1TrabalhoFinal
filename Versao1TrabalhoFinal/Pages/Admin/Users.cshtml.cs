using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class UsersModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly StandDbContext _context;

        public UsersModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            StandDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public List<UserRoleViewModel> UsersList { get; set; } = new();

        public List<string> AvailableRoles { get; set; } = new();

        [BindProperty]
        public string IdentityUserId { get; set; } = string.Empty;

        [BindProperty]
        public string SelectedRole { get; set; } = string.Empty;

        [BindProperty]
        public CreateUserInputModel NewUser { get; set; } = new();

        public async Task OnGetAsync()
        {
            await CarregarDadosAsync();
        }

        public async Task<IActionResult> OnPostCreateUserAsync()
        {
            await CarregarDadosAsync();

            ModelState.Remove(nameof(IdentityUserId));
            ModelState.Remove(nameof(SelectedRole));

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var existingIdentityUser = await _userManager.FindByEmailAsync(NewUser.Email);
            if (existingIdentityUser != null)
            {
                ModelState.AddModelError(string.Empty, "Já existe um utilizador com esse email.");
                return Page();
            }

            if (!await _roleManager.RoleExistsAsync(NewUser.Role))
            {
                ModelState.AddModelError(string.Empty, "A role selecionada não existe.");
                return Page();
            }

            var identityUser = new IdentityUser
            {
                UserName = NewUser.Email,
                Email = NewUser.Email,
                EmailConfirmed = true,
                PhoneNumber = NewUser.Telefone
            };

            var createResult = await _userManager.CreateAsync(identityUser, NewUser.Password);

            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return Page();
            }

            var roleResult = await _userManager.AddToRoleAsync(identityUser, NewUser.Role);

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(identityUser);

                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return Page();
            }

            if (NewUser.Role == "Cliente")
            {
                var existingCliente = await _context.Clientes
                    .FirstOrDefaultAsync(c => c.Email == NewUser.Email);

                if (existingCliente == null)
                {
                    var cliente = new Cliente
                    {
                        Nome = NewUser.Nome,
                        Email = NewUser.Email,
                        Telefone = NewUser.Telefone,
                        NIF = null,
                        Morada = null,
                        ImagemUrl = "/images/users/default-avatar.jpg"
                    };

                    _context.Clientes.Add(cliente);
                    await _context.SaveChangesAsync();
                }
            }

            TempData["SuccessMessage"] = "Utilizador criado com sucesso.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostChangeRoleAsync()
        {
            await CarregarDadosAsync();

            ModelState.Remove("NewUser.Nome");
            ModelState.Remove("NewUser.Email");
            ModelState.Remove("NewUser.Telefone");
            ModelState.Remove("NewUser.Password");
            ModelState.Remove("NewUser.ConfirmPassword");
            ModelState.Remove("NewUser.Role");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var identityUser = await _userManager.FindByIdAsync(IdentityUserId);

            if (identityUser == null)
            {
                ModelState.AddModelError(string.Empty, "Utilizador não encontrado.");
                return Page();
            }

            var currentRoles = await _userManager.GetRolesAsync(identityUser);

            if (currentRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(identityUser, currentRoles);

                if (!removeResult.Succeeded)
                {
                    foreach (var error in removeResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return Page();
                }
            }

            if (!string.IsNullOrWhiteSpace(SelectedRole))
            {
                if (!await _roleManager.RoleExistsAsync(SelectedRole))
                {
                    ModelState.AddModelError(string.Empty, "A role selecionada não existe.");
                    return Page();
                }

                var addRoleResult = await _userManager.AddToRoleAsync(identityUser, SelectedRole);

                if (!addRoleResult.Succeeded)
                {
                    foreach (var error in addRoleResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return Page();
                }
            }

            TempData["SuccessMessage"] = "Role atualizada com sucesso.";
            return RedirectToPage();
        }

        private async Task CarregarDadosAsync()
        {
            AvailableRoles = await _roleManager.Roles
                .Select(r => r.Name!)
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .OrderBy(r => r)
                .ToListAsync();

            var identityUsers = await _userManager.Users
                .OrderBy(u => u.Email)
                .ToListAsync();

            var clientes = await _context.Clientes
                .AsNoTracking()
                .ToListAsync();

            UsersList = new List<UserRoleViewModel>();

            foreach (var identityUser in identityUsers)
            {
                var roles = await _userManager.GetRolesAsync(identityUser);
                var currentRole = roles.FirstOrDefault() ?? "Sem Role";

                var cliente = clientes.FirstOrDefault(c => c.Email == identityUser.Email);

                UsersList.Add(new UserRoleViewModel
                {
                    IdentityUserId = identityUser.Id,
                    Nome = cliente?.Nome ?? identityUser.UserName ?? "Sem nome",
                    Email = identityUser.Email ?? string.Empty,
                    Telefone = cliente?.Telefone ?? identityUser.PhoneNumber,
                    CurrentRole = currentRole,
                    Ativo = true
                });
            }
        }

        public class UserRoleViewModel
        {
            public string IdentityUserId { get; set; } = string.Empty;
            public string Nome { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string? Telefone { get; set; }
            public string CurrentRole { get; set; } = string.Empty;
            public bool Ativo { get; set; }
        }

        public class CreateUserInputModel
        {
            [Required(ErrorMessage = "O nome é obrigatório.")]
            [Display(Name = "Nome")]
            public string Nome { get; set; } = string.Empty;

            [Required(ErrorMessage = "O email é obrigatório.")]
            [EmailAddress(ErrorMessage = "Introduza um email válido.")]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Display(Name = "Telefone")]
            public string? Telefone { get; set; }

            [Required(ErrorMessage = "A password é obrigatória.")]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "A confirmação da password é obrigatória.")]
            [DataType(DataType.Password)]
            [Display(Name = "Confirmar password")]
            [Compare(nameof(Password), ErrorMessage = "A password e a confirmação não coincidem.")]
            public string ConfirmPassword { get; set; } = string.Empty;

            [Required(ErrorMessage = "Selecione uma role.")]
            [Display(Name = "Role")]
            public string Role { get; set; } = string.Empty;
        }
    }
}
