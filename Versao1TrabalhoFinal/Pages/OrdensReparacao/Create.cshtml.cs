using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.OrdensReparacao
{
    /// <summary>
    /// Página responsável pela criação de ordens de reparação.
    /// </summary>
    [Authorize(Roles = "Admin,Colaborador")]
    public class CreateModel : PageModel
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Construtor da página de criação.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public CreateModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Ordem de reparação ligada ao formulário.
        /// </summary>
        [BindProperty]
        public OrdemReparacao OrdemReparacao { get; set; } = new();

        /// <summary>
        /// Lista de clientes para a dropdown.
        /// </summary>
        public SelectList ClientesSelect { get; set; } = default!;

        /// <summary>
        /// Lista de veículos para a dropdown.
        /// </summary>
        public SelectList VeiculosSelect { get; set; } = default!;

        /// <summary>
        /// Carrega os dados iniciais do formulário.
        /// </summary>
        public void OnGet()
        {
            OrdemReparacao.DataEntrada = DateTime.Now;

            ClientesSelect = new SelectList(_context.Clientes.OrderBy(c => c.Nome).ToList(), "Id", "Nome");
            VeiculosSelect = new SelectList(_context.Veiculos.OrderBy(v => v.Marca).ThenBy(v => v.Modelo).ToList(), "Id", "Modelo");
        }

        /// <summary>
        /// Guarda a nova ordem de reparação.
        /// </summary>
        /// <returns>Redireciona para a listagem ou volta à página em caso de erro.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ClientesSelect = new SelectList(_context.Clientes.OrderBy(c => c.Nome).ToList(), "Id", "Nome");
                VeiculosSelect = new SelectList(_context.Veiculos.OrderBy(v => v.Marca).ThenBy(v => v.Modelo).ToList(), "Id", "Modelo");
                return Page();
            }

            _context.OrdensReparacao.Add(OrdemReparacao);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
