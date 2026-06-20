using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;
using ClienteEntity = Versao1TrabalhoFinal.Models.Cliente;

namespace Versao1TrabalhoFinal.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly StandDbContext _context;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            StandDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        public class InputModel
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
            [StringLength(100, ErrorMessage = "A {0} deve ter pelo menos {2} e no máximo {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "A confirmação da password é obrigatória.")]
            [DataType(DataType.Password)]
            [Display(Name = "Confirmar password")]
            [Compare("Password", ErrorMessage = "A password não coincide.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public IActionResult OnGet(string? returnUrl = null)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Index");
            }

            ReturnUrl = returnUrl;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var existingIdentityUser = await _userManager.FindByEmailAsync(Input.Email);

            if (existingIdentityUser != null)
            {
                ModelState.AddModelError(string.Empty, "Já existe uma conta com esse email.");
                return Page();
            }

            var existingCliente = await _context.Clientes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Email == Input.Email);

            if (existingCliente != null)
            {
                ModelState.AddModelError(string.Empty, "Já existe um cliente com esse email.");
                return Page();
            }

            if (!await _roleManager.RoleExistsAsync("Cliente"))
            {
                var createRoleResult = await _roleManager.CreateAsync(new IdentityRole("Cliente"));

                if (!createRoleResult.Succeeded)
                {
                    foreach (var error in createRoleResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return Page();
                }
            }

            var identityUser = new IdentityUser
            {
                UserName = Input.Email,
                Email = Input.Email,
                EmailConfirmed = true,
                PhoneNumber = Input.Telefone
            };

            var createUserResult = await _userManager.CreateAsync(identityUser, Input.Password);

            if (!createUserResult.Succeeded)
            {
                foreach (var error in createUserResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return Page();
            }

            var addRoleResult = await _userManager.AddToRoleAsync(identityUser, "Cliente");

            if (!addRoleResult.Succeeded)
            {
                await _userManager.DeleteAsync(identityUser);

                foreach (var error in addRoleResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return Page();
            }

            var cliente = new ClienteEntity
            {
                Nome = Input.Nome,
                Email = Input.Email,
                Telefone = Input.Telefone,
                NIF = null,
                Morada = null,
                ImagemUrl = "/images/users/default-avatar.jpg",
                IdentityUserId = identityUser.Id
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            await _signInManager.SignInAsync(identityUser, isPersistent: false);

            if (!string.IsNullOrWhiteSpace(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            {
                return LocalRedirect(ReturnUrl);
            }

            return RedirectToPage("/ClienteArea/Veiculos");
        }
    }
}