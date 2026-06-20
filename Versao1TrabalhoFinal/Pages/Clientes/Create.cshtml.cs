using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;
using ClienteEntity = Versao1TrabalhoFinal.Models.Cliente;

namespace Versao1TrabalhoFinal.Pages.Clientes
{
    /// <summary>
    /// Página responsável pela criação de um novo cliente e da respetiva conta Identity associada.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly StandDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Inicializa uma nova instância da página de criação de clientes.
        /// </summary>
        /// <param name="context">Contexto da base de dados da aplicação.</param>
        /// <param name="userManager">Serviço de gestão de utilizadores do ASP.NET Core Identity.</param>
        public CreateModel(StandDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Dados introduzidos no formulário de criação.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; } = new();

        /// <summary>
        /// Carrega a página de criação.
        /// </summary>
        /// <returns>Resultado da execução da página.</returns>
        public IActionResult OnGet()
        {
            return Page();
        }

        /// <summary>
        /// Processa a submissão do formulário e cria o cliente e o utilizador Identity correspondente.
        /// </summary>
        /// <returns>Redireciona para a listagem em caso de sucesso; caso contrário, devolve a página com erros de validação.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var emailExistsInClientes = await _context.Clientes
                .AnyAsync(c => c.Email == Input.Email);

            if (emailExistsInClientes)
            {
                ModelState.AddModelError("Input.Email", "Já existe um cliente com este email.");
                return Page();
            }

            var existingUser = await _userManager.FindByEmailAsync(Input.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError("Input.Email", "Já existe uma conta associada a este email.");
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

            var roleResult = await _userManager.AddToRoleAsync(user, "Cliente");

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);

                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return Page();
            }

            var cliente = new ClienteEntity
            {
                Nome = Input.Nome,
                Telefone = Input.Telefone,
                Email = Input.Email,
                NIF = Input.NIF,
                Morada = Input.Morada,
                ImagemUrl = Input.ImagemUrl,
                IdentityUserId = user.Id
            };

            try
            {
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();
            }
            catch
            {
                await _userManager.DeleteAsync(user);
                ModelState.AddModelError(string.Empty, "Ocorreu um erro ao guardar o cliente.");
                return Page();
            }

            TempData["SuccessMessage"] = "Cliente criado com sucesso.";
            return RedirectToPage("./Index");
        }

        /// <summary>
        /// Modelo de dados utilizado no formulário de criação de clientes.
        /// </summary>
        public class InputModel
        {
            [Required(ErrorMessage = "O nome é obrigatório.")]
            [StringLength(100)]
            [Display(Name = "Nome")]
            public string Nome { get; set; } = string.Empty;

            [Phone(ErrorMessage = "Introduza um número de telefone válido.")]
            [Display(Name = "Telefone")]
            public string? Telefone { get; set; }

            [Required(ErrorMessage = "O email é obrigatório.")]
            [EmailAddress(ErrorMessage = "Introduza um email válido.")]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "A palavra-passe é obrigatória.")]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "A palavra-passe deve ter entre 6 e 100 caracteres.")]
            [DataType(DataType.Password)]
            [Display(Name = "Palavra-passe")]
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "A confirmação da palavra-passe é obrigatória.")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "A palavra-passe e a confirmação não coincidem.")]
            [Display(Name = "Confirmar palavra-passe")]
            public string ConfirmPassword { get; set; } = string.Empty;

            [Display(Name = "NIF")]
            [StringLength(20)]
            public string? NIF { get; set; }

            [Display(Name = "Morada")]
            [StringLength(250)]
            public string? Morada { get; set; }

            [Display(Name = "Imagem")]
            [StringLength(500)]
            public string? ImagemUrl { get; set; }
        }
    }
}