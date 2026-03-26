using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;
using Versao1TrabalhoFinal.Services;

namespace Versao1TrabalhoFinal.Pages.GestaoOrcamentos
{
    /// <summary>
    /// Página para adicionar itens a um orçamento.
    /// </summary>
    [Authorize(Roles = "Admin,Mecanico,Rececionista")]
    public class AddItemModel : PageModel
    {
        private readonly StandDbContext _context;
        private readonly OrcamentoService _orcamentoService;

        /// <summary>
        /// Inicializa a página.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        /// <param name="orcamentoService">Serviço de apoio aos orçamentos.</param>
        public AddItemModel(StandDbContext context, OrcamentoService orcamentoService)
        {
            _context = context;
            _orcamentoService = orcamentoService;
        }

        /// <summary>
        /// Identificador do orçamento.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public int OrcamentoId { get; set; }

        /// <summary>
        /// Item a adicionar ao orçamento.
        /// </summary>
        [BindProperty]
        public OrcamentoItem Item { get; set; } = new();

        /// <summary>
        /// Título do orçamento apresentado na página.
        /// </summary>
        public string TituloOrcamento { get; set; } = string.Empty;

        /// <summary>
        /// Carrega a página.
        /// </summary>
        /// <returns>Página de adição de item.</returns>
        public async Task<IActionResult> OnGetAsync()
        {
            var orcamento = await _context.Orcamentos.FindAsync(OrcamentoId);

            if (orcamento == null)
                return NotFound();

            TituloOrcamento = $"Orçamento #{orcamento.Id}";
            return Page();
        }

        /// <summary>
        /// Adiciona o item ao orçamento.
        /// </summary>
        /// <returns>Redireciona para o detalhe do orçamento.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            var orcamento = await _context.Orcamentos
                .Include(o => o.Descricao)
                .FirstOrDefaultAsync(o => o.Id == OrcamentoId);

            if (orcamento == null)
                return NotFound();

            if (!ModelState.IsValid)
                return Page();

            Item.OrcamentoId = OrcamentoId;

            //orcamento.ValorEstimado.Add(Item);

            _orcamentoService.RecalcularValorEstimado(orcamento);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Details", new { id = OrcamentoId });
        }
    }
}
