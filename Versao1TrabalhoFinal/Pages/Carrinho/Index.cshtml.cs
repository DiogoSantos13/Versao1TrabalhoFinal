using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Carrinho
{
    /// <summary>
    /// Página responsável por apresentar o conteúdo do carrinho do cliente autenticado.
    /// </summary>
    [Authorize(Roles = "Cliente")]
    public class IndexModel : PageModel
    {
        private readonly StandDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Inicializa uma nova instância da página do carrinho.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        /// <param name="userManager">Gestor de utilizadores do Identity.</param>
        public IndexModel(StandDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Carrinho do cliente autenticado.
        /// </summary>
        public Versao1TrabalhoFinal.Models.Carrinho? Carrinho { get; set; }

        /// <summary>
        /// Total monetário dos produtos presentes no carrinho.
        /// </summary>
        public decimal TotalProdutos { get; set; }

        /// <summary>
        /// Total monetário dos serviços presentes no carrinho.
        /// </summary>
        public decimal TotalServicos { get; set; }

        /// <summary>
        /// Total monetário dos veículos do stand presentes no carrinho.
        /// </summary>
        public decimal TotalVeiculosStand { get; set; }

        /// <summary>
        /// Total global do carrinho.
        /// </summary>
        public decimal TotalGeral { get; set; }

        /// <summary>
        /// Indica se o carrinho existe e contém pelo menos um item.
        /// </summary>
        public bool TemItens =>
            Carrinho != null &&
            (
                Carrinho.Produtos.Any() ||
                Carrinho.Servicos.Any() ||
                Carrinho.CarrinhoVeiculosStand.Any()
            );

        /// <summary>
        /// Carrega o carrinho do cliente autenticado e os respetivos itens.
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

            Carrinho = await _context.Carrinhos
                .AsNoTracking()
                .Include(c => c.Produtos)
                    .ThenInclude(cp => cp.Produto)
                .Include(c => c.Servicos)
                    .ThenInclude(cs => cs.Servico)
                .Include(c => c.CarrinhoVeiculosStand)
                    .ThenInclude(cvs => cvs.VeiculoStand)
                        .ThenInclude(vs => vs.Veiculo)
                .FirstOrDefaultAsync(c => c.ClienteId == cliente.Id);

            if (Carrinho == null)
            {
                return;
            }

            TotalProdutos = Carrinho.Produtos.Sum(p => p.PrecoNoMomento);
            TotalServicos = Carrinho.Servicos.Sum(s => s.PrecoNoMomento);
            TotalVeiculosStand = Carrinho.CarrinhoVeiculosStand.Sum(v => v.PrecoNoMomento);
            TotalGeral = TotalProdutos + TotalServicos + TotalVeiculosStand;
        }
    }
}