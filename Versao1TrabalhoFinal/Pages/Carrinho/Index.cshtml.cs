using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Carrinho
{
    /// <summary>
    /// Página principal do carrinho do cliente.
    /// Apresenta os produtos e serviços atualmente associados ao carrinho.
    /// </summary>
    [Authorize(Roles = "Cliente")]
    public class IndexModel : PageModel
    {
        private readonly StandDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Inicializa uma nova instância da página principal do carrinho.
        /// </summary>
        public IndexModel(StandDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Itens de serviços presentes no carrinho.
        /// </summary>
        public IList<CarrinhoServico> ItensServicos { get; set; } = new List<CarrinhoServico>();

        /// <summary>
        /// Itens de produtos presentes no carrinho.
        /// </summary>
        public IList<CarrinhoProdutos> ItensProdutos { get; set; } = new List<CarrinhoProdutos>();

        /// <summary>
        /// Total atual do carrinho.
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Carrega os dados do carrinho do cliente autenticado.
        /// </summary>
        public async Task OnGetAsync()
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return;
            }

            var cliente = await _context.Clientes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.IdentityUserId == userId);

            if (cliente == null)
            {
                return;
            }

            var carrinho = await _context.Carrinhos
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ClienteId == cliente.Id);

            if (carrinho == null)
            {
                return;
            }

            ItensServicos = await _context.CarrinhoServicos
                .AsNoTracking()
                .Where(cs => cs.CarrinhoId == carrinho.Id)
                .Include(cs => cs.Servico)
                .OrderByDescending(cs => cs.DataAdicao)
                .ToListAsync();

            ItensProdutos = await _context.CarrinhoProdutos
                .AsNoTracking()
                .Where(cp => cp.CarrinhoId == carrinho.Id)
                .Include(cp => cp.Produto)
                .OrderByDescending(cp => cp.DataAdicao)
                .ToListAsync();

            Total = ItensServicos.Sum(s => s.PrecoNoMomento)
                + ItensProdutos.Sum(p => p.PrecoNoMomento * p.Quantidade);
        }
    }
}