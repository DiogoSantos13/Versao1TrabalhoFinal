using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Veiculos
{
    /// <summary>
    /// Página responsável pela eliminação de veículos.
    /// </summary>
    [Authorize(Roles = "Admin,Colaborador")]
    public class DeleteModel : PageModel
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Construtor da página de eliminação.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public DeleteModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Veículo a apresentar e eliminar.
        /// </summary>
        [BindProperty]
        public Veiculo Veiculo { get; set; } = new();

        /// <summary>
        /// Carrega o veículo antes da confirmação de eliminação.
        /// </summary>
        /// <param name="id">Id do veículo.</param>
        /// <returns>Página ou NotFound.</returns>
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var veiculo = await _context.Veiculos
                .Include(v => v.Cliente)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (veiculo == null)
                return NotFound();

            Veiculo = veiculo;
            return Page();
        }

        /// <summary>
        /// Elimina o veículo selecionado.
        /// </summary>
        /// <param name="id">Id do veículo.</param>
        /// <returns>Redireciona para a listagem.</returns>
        public async Task<IActionResult> OnPostAsync(int id)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);

            if (veiculo != null)
            {
                _context.Veiculos.Remove(veiculo);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
