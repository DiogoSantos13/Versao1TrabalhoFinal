using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Clientes
{
    /// <summary>
    /// Página responsável pela edição dos dados de um cliente e da conta Identity associada.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly StandDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Inicializa uma nova instância da página de edição de clientes.
        /// </summary>
        /// <param name="context">Contexto da base de dados da aplicação.</param>
        /// <param name="userManager">Serviço de gestão de utilizadores do ASP.NET Core Identity.</param>
        public EditModel(StandDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Dados utilizados no formulário de edição.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; } = new();

        /// <summary>
        /// Carrega os dados do cliente para edição.
        /// </summary>
        /// <param name="id">Identificador do cliente.</param>
        /// <returns>Resultado da execução da página.</returns>
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);

            if (cliente == null)
            {
                return NotFound();
            }

            Input = new InputModel
            {
                Id = cliente.Id,
                IdentityUserId = cliente.IdentityUserId,
                Nome = cliente.Nome,
                Telefone = cliente.Telefone,
                Email = cliente.Email,
                NIF = cliente.NIF,
                Morada = cliente.Morada,
                ImagemUrl = cliente.ImagemUrl
            };

            return Page();
        }

        /// <summary>
        /// Processa a submissão do formulário e atualiza os dados do cliente e da conta associada.
        /// </summary>
        /// <returns>Redireciona para a listagem em caso de sucesso; caso contrário, devolve a página com erros.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == Input.Id);

            if (cliente == null)
            {
                return NotFound();
            }

            var emailExistsInClientes = await _context.Clientes
                .AnyAsync(c => c.Email == Input.Email && c.Id != Input.Id);

            if (emailExistsInClientes)
            {
                ModelState.AddModelError("Input.Email", "Já existe outro cliente com este email.");
                return Page();
            }

            IdentityUser? user = null;

            if (!string.IsNullOrWhiteSpace(cliente.IdentityUserId))
            {
                user = await _userManager.FindByIdAsync(cliente.IdentityUserId);
            }

            if (user != null)
            {
                var existingUserWithEmail = await _userManager.FindByEmailAsync(Input.Email);
                if (existingUserWithEmail != null && existingUserWithEmail.Id != user.Id)
                {
                    ModelState.AddModelError("Input.Email", "Já existe outra conta com este email.");
                    return Page();
                }
            }

            cliente.Nome = Input.Nome;
            cliente.Telefone = Input.Telefone;
            cliente.Email = Input.Email;
            cliente.NIF = Input.NIF;
            cliente.Morada = Input.Morada;
            cliente.ImagemUrl = Input.ImagemUrl;

            if (user != null)
            {
                user.Email = Input.Email;
                user.UserName = Input.Email;
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
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cliente atualizado com sucesso.";
            return RedirectToPage("./Index");
        }

        /// <summary>
        /// Modelo de dados utilizado no formulário de edição de clientes.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            /// Identificador do cliente.
            /// </summary>
            public int Id { get; set; }

            /// <summary>
            /// Identificador do utilizador Identity associado ao cliente.
            /// </summary>
            public string? IdentityUserId { get; set; }

            /// <summary>
            /// Nome do cliente.
            /// </summary>
            [Required(ErrorMessage = "O nome é obrigatório.")]
            [StringLength(100)]
            [Display(Name = "Nome")]
            public string Nome { get; set; } = string.Empty;

            /// <summary>
            /// Telefone do cliente.
            /// </summary>
            [Phone(ErrorMessage = "Introduza um número de telefone válido.")]
            [Display(Name = "Telefone")]
            public string? Telefone { get; set; }

            /// <summary>
            /// Email do cliente e da conta associada.
            /// </summary>
            [Required(ErrorMessage = "O email é obrigatório.")]
            [EmailAddress(ErrorMessage = "Introduza um email válido.")]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            /// <summary>
            /// Número de identificação fiscal do cliente.
            /// </summary>
            [Display(Name = "NIF")]
            [StringLength(20)]
            public string? NIF { get; set; }

            /// <summary>
            /// Morada do cliente.
            /// </summary>
            [Display(Name = "Morada")]
            [StringLength(250)]
            public string? Morada { get; set; }

            /// <summary>
            /// URL da imagem do cliente.
            /// </summary>
            [Display(Name = "Imagem")]
            [StringLength(500)]
            public string? ImagemUrl { get; set; }
        }
    }
}
