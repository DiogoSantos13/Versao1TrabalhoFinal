using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.OrdensReparacao
{
    /// <summary>
    /// Página responsável pela edição de ordens de reparação.
    /// </summary>
    [Authorize(Roles = "Admin,Colaborador")]
    public class EditModel : PageModel
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Construtor da página.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public EditModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Ordem de reparação ligada ao formulário.
        /// </summary>
        [BindProperty]
        public OrdemReparacao OrdemReparacao { get; set; } = new();

        /// <summary>
        /// Dropdown de clientes.
        /// </summary>
        public SelectList ClientesSelect { get; set; } = default!;

        /// <summary>
        /// Dropdown de veículos.
        /// </summary>
        public SelectList VeiculosSelect { get; set; } = default!;

        /// <summary>
        /// Carrega os dados da ordem para edição.
        /// </summary>
        /// <param name="id">Id da ordem.</param>
        /// <returns>Página ou NotFound.</returns>
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var ordem = await _context.OrdensReparacao.FindAsync(id);

            if (ordem == null)
                return NotFound();

            OrdemReparacao = ordem;

            ClientesSelect = new SelectList(await _context.Clientes.OrderBy(c => c.Nome).ToListAsync(), "Id", "Nome", OrdemReparacao.ClienteId);
            VeiculosSelect = new SelectList(await _context.Veiculos.OrderBy(v => v.Marca).ThenBy(v => v.Modelo).ToListAsync(), "Id", "Modelo", OrdemReparacao.VeiculoId);

            return Page();
        }

        /// <summary>
        /// Guarda as alterações da ordem.
        /// </summary>
        /// <returns>Redireciona para a listagem ou volta à página se houver erro.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ClientesSelect = new SelectList(await _context.Clientes.OrderBy(c => c.Nome).ToListAsync(), "Id", "Nome", OrdemReparacao.ClienteId);
                VeiculosSelect = new SelectList(await _context.Veiculos.OrderBy(v => v.Marca).ThenBy(v => v.Modelo).ToListAsync(), "Id", "Modelo", OrdemReparacao.VeiculoId);
                return Page();
            }

            _context.Attach(OrdemReparacao).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
