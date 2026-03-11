using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.ClienteArea
{
    /// <summary>
    /// Página onde o cliente cria um novo pedido de orçamento.
    /// </summary>
    [Authorize(Roles = "Cliente")]
    public class NovoPedidoOrcamentoModel : PageModel
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Construtor da página.
        /// </summary>
        public NovoPedidoOrcamentoModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Pedido de orçamento ligado ao formulário.
        /// </summary>
        [BindProperty]
        public Orcamento Orcamento { get; set; } = new();

        /// <summary>
        /// Lista de veículos do cliente autenticado.
        /// </summary>
        public SelectList VeiculosSelect { get; set; } = default!;

        /// <summary>
        /// Carrega os veículos do cliente atual.
        /// </summary>
        public async Task<IActionResult> OnGetAsync()
        {
            var email = User.Identity?.Name;
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == email);

            if (cliente == null)
                return RedirectToPage("/Index");

            var veiculos = await _context.Veiculos
                .Where(v => v.ClienteId == cliente.Id)
                .OrderBy(v => v.Marca)
                .ThenBy(v => v.Modelo)
                .ToListAsync();

            VeiculosSelect = new SelectList(veiculos, "Id", "Modelo");
            return Page();
        }

        /// <summary>
        /// Cria o novo pedido de orçamento.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            var email = User.Identity?.Name;
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == email);

            if (cliente == null)
                return RedirectToPage("/Index");

            if (!ModelState.IsValid)
            {
                var veiculos = await _context.Veiculos
                    .Where(v => v.ClienteId == cliente.Id)
                    .OrderBy(v => v.Marca)
                    .ThenBy(v => v.Modelo)
                    .ToListAsync();

                VeiculosSelect = new SelectList(veiculos, "Id", "Modelo");
                return Page();
            }

            Orcamento.ClienteId = cliente.Id;
            Orcamento.DataCriacao = DateTime.Now;
            Orcamento.Estado = "Pedido Submetido";
            Orcamento.GeradoPorIA = false;
            Orcamento.ValorEstimado = 0;

            _context.Orcamentos.Add(Orcamento);
            await _context.SaveChangesAsync();

            return RedirectToPage("/ClienteArea/Orcamentos");
        }
    }
}
