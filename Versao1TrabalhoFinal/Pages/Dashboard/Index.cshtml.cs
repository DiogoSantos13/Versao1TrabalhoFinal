using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;

namespace Versao1TrabalhoFinal.Pages.Dashboard
{
    /// <summary>
    /// Página principal de dashboard para administradores.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Inicializa a página de dashboard.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public IndexModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Número total de clientes.
        /// </summary>
        public int TotalClientes { get; set; }

        /// <summary>
        /// Número total de veículos.
        /// </summary>
        public int TotalVeiculos { get; set; }

        /// <summary>
        /// Número total de produtos.
        /// </summary>
        public int TotalProdutos { get; set; }

        /// <summary>
        /// Número total de serviços.
        /// </summary>
        public int TotalServicos { get; set; }

        /// <summary>
        /// Número de ordens abertas.
        /// </summary>
        public int OrdensAbertas { get; set; }

        /// <summary>
        /// Carrega os indicadores principais do dashboard.
        /// </summary>
        public async Task OnGetAsync()
        {
            TotalClientes = await _context.Clientes.CountAsync();
            TotalVeiculos = await _context.Veiculos.CountAsync();
            TotalProdutos = await _context.Produtos.CountAsync();
            TotalServicos = await _context.Servicos.CountAsync();
            OrdensAbertas = await _context.OrdensReparacao.CountAsync(o => o.Estado == "Aberta");
        }
    }
}
