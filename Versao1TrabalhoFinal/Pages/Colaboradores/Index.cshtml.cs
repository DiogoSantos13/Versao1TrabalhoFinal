using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;

namespace Versao1TrabalhoFinal.Pages.Colaboradores
{
    [Authorize(Roles = "Admin,Colaborador")]
    public class IndexModel : PageModel
    {
        private readonly StandDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(
            StandDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public string? Role { get; set; }

        public List<SelectListItem> RolesFiltro { get; set; } = new();

        public IList<ColaboradorViewModel> Colaboradores { get; set; } = new List<ColaboradorViewModel>();

        public async Task OnGetAsync()
        {
            await CarregarRolesFiltroAsync();

            var colaboradores = await _context.Colaboradores
                .OrderBy(c => c.Nome)
                .ToListAsync();

            var lista = new List<ColaboradorViewModel>();

            foreach (var colaborador in colaboradores)
            {
                var user = await _userManager.FindByIdAsync(colaborador.IdentityUserId);
                var roles = user != null
                    ? await _userManager.GetRolesAsync(user)
                    : new List<string>();

                if (!string.IsNullOrWhiteSpace(Role) &&
                    !roles.Any(r => string.Equals(r, Role, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                lista.Add(new ColaboradorViewModel
                {
                    Id = colaborador.Id,
                    Nome = colaborador.Nome,
                    Email = colaborador.Email,
                    Telefone = colaborador.Telefone,
                    Cargo = colaborador.Cargo,
                    Roles = roles.ToList()
                });
            }

            Colaboradores = lista;
        }

        private async Task CarregarRolesFiltroAsync()
        {
            RolesFiltro = await _context.Roles
                .OrderBy(r => r.Name)
                .Select(r => new SelectListItem
                {
                    Value = r.Name!,
                    Text = r.Name!
                })
                .ToListAsync();

            RolesFiltro.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "Todas"
            });
        }

        public class ColaboradorViewModel
        {
            public int Id { get; set; }
            public string Nome { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string? Telefone { get; set; }
            public string? Cargo { get; set; }
            public List<string> Roles { get; set; } = new();
        }
    }
}