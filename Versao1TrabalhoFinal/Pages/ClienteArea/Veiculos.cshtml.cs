using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.ClienteArea
{
    [Authorize(Roles = "Cliente")]
    public class VeiculosModel : PageModel
    {
        private readonly StandDbContext _context;

        public VeiculosModel(StandDbContext context)
        {
            _context = context;
        }

        public List<Veiculo> Veiculos { get; set; } = new();

        /// <summary>
        /// Carrega os veículos associados ao cliente autenticado.
        /// </summary>
        public async Task OnGetAsync()
        {
            var email = User.Identity?.Name;

            if (string.IsNullOrWhiteSpace(email))
            {
                Veiculos = new List<Veiculo>();
                return;
            }

            var cliente = await _context.Clientes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Email == email);

            if (cliente == null)
            {
                Veiculos = new List<Veiculo>();
                return;
            }

            Veiculos = await _context.Veiculos
                .Where(v => v.ClienteId == cliente.Id)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
