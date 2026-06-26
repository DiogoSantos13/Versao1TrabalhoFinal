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
    public class CreateModel : PageModel
    {
        private readonly StandDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CreateModel(
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

        public async Task OnGetAsync()
        {
            await LoadRolesAsync();
            Input.ImagemUrl = "/images/users/default-avatar.jpg";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadRolesAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var roleExiste = await _roleManager.RoleExistsAsync(Input.Role);
            if (!roleExiste)
            {
                ModelState.AddModelError("Input.Role", "A role selecionada não existe.");
                return Page();
            }

            var emailExisteNoIdentity = await _userManager.FindByEmailAsync(Input.Email);
            if (emailExisteNoIdentity != null)
            {
                ModelState.AddModelError("Input.Email", "Já existe uma conta com este email.");
                return Page();
            }

            var emailExisteEmClientes = await _context.Clientes.AnyAsync(c => c.Email == Input.Email);
            if (emailExisteEmClientes)
            {
                ModelState.AddModelError("Input.Email", "Já existe um cliente com este email.");
                return Page();
            }

            var emailExisteEmColaboradores = await _context.Colaboradores.AnyAsync(c => c.Email == Input.Email);
            if (emailExisteEmColaboradores)
            {
                ModelState.AddModelError("Input.Email", "Já existe um colaborador com este email.");
                return Page();
            }

            var user = new IdentityUser
            {
                UserName = Input.Email,
                Email = Input.Email,
                PhoneNumber = Input.Telefone,
                EmailConfirmed = true
            };

            var createUserResult = await _userManager.CreateAsync(user, Input.Password);
            if (!createUserResult.Succeeded)
            {
                foreach (var error in createUserResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return Page();
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, Input.Role);
            if (!addRoleResult.Succeeded)
            {
                foreach (var error in addRoleResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                await _userManager.DeleteAsync(user);
                return Page();
            }

            if (Input.Role != "Colaborador" && await _roleManager.RoleExistsAsync("Colaborador"))
            {
                await _userManager.AddToRoleAsync(user, "Colaborador");
            }

            var colaborador = new Colaborador
            {
                IdentityUserId = user.Id,
                Nome = Input.Nome,
                Telefone = Input.Telefone,
                Email = Input.Email,
                NIF = Input.NIF,
                Morada = Input.Morada,
                ImagemUrl = string.IsNullOrWhiteSpace(Input.ImagemUrl)
                    ? "/images/users/default-avatar.jpg"
                    : Input.ImagemUrl,
                Cargo = Input.Cargo
            };

            _context.Colaboradores.Add(colaborador);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Colaborador criado com sucesso.";
            return RedirectToPage("./Index");
        }

        private async Task LoadRolesAsync()
        {
            var rolesPermitidas = new[] { "Colaborador", "Funcionario", "Rececionista", "Vendedor", "Mecanico", "Admin" };

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

        public class InputModel
        {
            [Required(ErrorMessage = "O nome é obrigatório.")]
            [StringLength(100)]
            public string Nome { get; set; } = string.Empty;

            [Phone(ErrorMessage = "Introduza um número de telefone válido.")]
            public string? Telefone { get; set; }

            [Required(ErrorMessage = "O email é obrigatório.")]
            [EmailAddress(ErrorMessage = "Introduza um email válido.")]
            [StringLength(150)]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "A password é obrigatória.")]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "A password deve ter entre 6 e 100 caracteres.")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "A confirmação da password é obrigatória.")]
            [Compare("Password", ErrorMessage = "As passwords não coincidem.")]
            [DataType(DataType.Password)]
            public string ConfirmPassword { get; set; } = string.Empty;

            [StringLength(20)]
            public string? NIF { get; set; }

            [StringLength(250)]
            public string? Morada { get; set; }

            [StringLength(500)]
            public string? ImagemUrl { get; set; }

            [StringLength(100)]
            public string? Cargo { get; set; }

            [Required(ErrorMessage = "Selecione uma role.")]
            public string Role { get; set; } = string.Empty;
        }
    }
}