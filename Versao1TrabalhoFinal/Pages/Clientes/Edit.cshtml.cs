using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;

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
            await LoadRolesAsync();

            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);

            if (cliente == null)
            {
                return NotFound();
            }

            string roleAtual = "Cliente";

            if (!string.IsNullOrWhiteSpace(cliente.IdentityUserId))
            {
                var user = await _userManager.FindByIdAsync(cliente.IdentityUserId);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    roleAtual = roles.FirstOrDefault() ?? "Cliente";
                }
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
                ImagemUrl = cliente.ImagemUrl,
                Role = roleAtual
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadRolesAsync();

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

                if (!string.IsNullOrWhiteSpace(Input.Role))
                {
                    var roleExiste = await _roleManager.RoleExistsAsync(Input.Role);
                    if (!roleExiste)
                    {
                        ModelState.AddModelError("Input.Role", "A role selecionada não existe.");
                        return Page();
                    }

                    var rolesAtuais = await _userManager.GetRolesAsync(user);

                    if (rolesAtuais.Any())
                    {
                        var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, rolesAtuais);
                        if (!removeRolesResult.Succeeded)
                        {
                            foreach (var error in removeRolesResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }

                            return Page();
                        }
                    }

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
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cliente atualizado com sucesso.";
            return RedirectToPage("./Index");
        }

        private async Task LoadRolesAsync()
        {
            RolesDisponiveis = await _roleManager.Roles
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
            public int Id { get; set; }

            public string? IdentityUserId { get; set; }

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

            [Display(Name = "NIF")]
            [StringLength(20)]
            public string? NIF { get; set; }

            [Display(Name = "Morada")]
            [StringLength(250)]
            public string? Morada { get; set; }

            [Display(Name = "Imagem")]
            [StringLength(500)]
            public string? ImagemUrl { get; set; }

            [Display(Name = "Role")]
            public string Role { get; set; } = "Cliente";
        }
    }
}