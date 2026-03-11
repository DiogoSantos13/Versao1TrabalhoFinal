using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Veiculos
{
    /// <summary>
    /// Página responsável pela edição de veículos.
    /// Permite aos utilizadores com role Admin ou Colaborador alterar os dados de um veículo existente.
    /// </summary>
    [Authorize(Roles = "Admin,Colaborador")]
    public class EditModel : PageModel
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Construtor da página de edição de veículos.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public EditModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Entidade veículo ligada ao formulário.
        /// </summary>
        [BindProperty]
        public Veiculo Veiculo { get; set; } = new();

        /// <summary>
        /// Lista de clientes para preencher a dropdown.
        /// </summary>
        public SelectList ClientesSelect { get; set; } = default!;

        /// <summary>
        /// Carrega os dados do veículo para edição.
        /// </summary>
        /// <param name="id">Id do veículo.</param>
        /// <returns>Página ou NotFound.</returns>
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);

            if (veiculo == null)
                return NotFound();

            Veiculo = veiculo;

            ClientesSelect = new SelectList(
                await _context.Clientes.OrderBy(c => c.Nome).ToListAsync(),
                "Id",
                "Nome",
                Veiculo.ClienteId);

            return Page();
        }

        /// <summary>
        /// Guarda as alterações efetuadas no veículo.
        /// </summary>
        /// <returns>Redireciona para a listagem ou volta à página em caso de erro.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ClientesSelect = new SelectList(
                    await _context.Clientes.OrderBy(c => c.Nome).ToListAsync(),
                    "Id",
                    "Nome",
                    Veiculo.ClienteId);

                return Page();
            }

            _context.Attach(Veiculo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Veiculos.AnyAsync(v => v.Id == Veiculo.Id))
                    return NotFound();

                throw;
            }

            return RedirectToPage("./Index");
        }
    }
}
