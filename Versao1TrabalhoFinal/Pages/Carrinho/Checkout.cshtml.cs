using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Carrinho
{
    /// <summary>
    /// Página de checkout do carrinho.
    /// Apresenta o resumo final de produtos e serviços.
    /// </summary>
    [Authorize(Roles = "Cliente")]
    public class CheckoutModel : PageModel
    {
        private readonly StandDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Inicializa uma nova instância da página de checkout.
        /// </summary>
        public CheckoutModel(StandDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Lista de serviços presentes no checkout.
        /// </summary>
        public IList<CarrinhoServico> Servicos { get; set; } = new List<CarrinhoServico>();

        /// <summary>
        /// Lista de produtos presentes no checkout.
        /// </summary>
        public IList<CarrinhoProdutos> Produtos { get; set; } = new List<CarrinhoProdutos>();

        /// <summary>
        /// Total do checkout.
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Carrega os dados do checkout.
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

            Servicos = await _context.CarrinhoServicos
                .AsNoTracking()
                .Where(cs => cs.CarrinhoId == carrinho.Id)
                .Include(cs => cs.Servico)
                .OrderByDescending(cs => cs.DataAdicao)
                .ToListAsync();

            Produtos = await _context.CarrinhoProdutos
                .AsNoTracking()
                .Where(cp => cp.CarrinhoId == carrinho.Id)
                .Include(cp => cp.Produto)
                .OrderByDescending(cp => cp.DataAdicao)
                .ToListAsync();

            Total = Servicos.Sum(s => s.PrecoNoMomento)
                + Produtos.Sum(p => p.PrecoNoMomento * p.Quantidade);
        }
    }
}