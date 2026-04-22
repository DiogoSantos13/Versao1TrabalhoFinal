using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using CarrinhoEntity = Versao1TrabalhoFinal.Models.Carrinho;
using CarrinhoProdutoEntity = Versao1TrabalhoFinal.Models.CarrinhoProdutos;
using ClienteEntity = Versao1TrabalhoFinal.Models.Cliente;
using System.Collections.Generic;

namespace Versao1TrabalhoFinal.Pages.Carrinho
{
    /// <summary>
    /// Página de listagem de produtos com filtros, compra para clientes e gestão para administradores e colaboradores.
    /// </summary>
    [Authorize(Roles = "Cliente,Admin,Colaborador")]
    public class ProdutoModel : PageModel
    {
       /// <summary>
       /// 
       /// </summary>
        private readonly StandDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Inicializa uma nova instância da página de adição de produto ao carrinho.
        /// </summary>
        public ProdutoModel(StandDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Adiciona o produto ao carrinho do cliente autenticado.
        /// </summary>
        /// <param name="produtoId">Identificador do produto.</param>
        public async Task<IActionResult> OnGetAsync(int produtoId)
        {
            var cliente = await ObterClienteAutenticadoAsync();

            if (cliente == null)
            {
                TempData["ErrorMessage"] = "Cliente autenticado não encontrado.";
                return RedirectToPage("/Produtos/Index");
            }

            var produto = await _context.Produtos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == produtoId);

            if (produto == null)
            {
                TempData["ErrorMessage"] = "Produto não encontrado.";
                return RedirectToPage("/Produtos/Index");
            }

            if (produto.Stock <= 0)
            {
                TempData["ErrorMessage"] = "Produto sem stock disponível.";
                return RedirectToPage("/Produtos/Index");
            }

            var carrinho = await ObterOuCriarCarrinhoAsync(cliente.Id);

            var itemExistente = await _context.CarrinhoProdutos
                .FirstOrDefaultAsync(cp => cp.CarrinhoId == carrinho.Id && cp.ProdutoId == produto.Id);

            if (itemExistente == null)
            {
                var item = new CarrinhoProdutoEntity
                {
                    CarrinhoId = carrinho.Id,
                    ProdutoId = produto.Id,
                    Quantidade = 1,
                    PrecoNoMomento = produto.Preco,
                    DataAdicao = DateTime.Now
                };

                _context.CarrinhoProdutos.Add(item);
            }
            else
            {
                if (itemExistente.Quantidade >= produto.Stock)
                {
                    TempData["ErrorMessage"] = "Não existe stock suficiente para aumentar a quantidade.";
                    return RedirectToPage("/Carrinho/Index");
                }

                itemExistente.Quantidade += 1;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Produto adicionado ao carrinho com sucesso.";
            return RedirectToPage("/Carrinho/Index");
        }

        /// <summary>
        /// Obtém o cliente autenticado associado ao utilizador atual.
        /// </summary>
        private async Task<ClienteEntity?> ObterClienteAutenticadoAsync()
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            return await _context.Clientes
                .FirstOrDefaultAsync(c => c.IdentityUserId == userId);
        }

        /// <summary>
        /// Obtém o carrinho do cliente ou cria um novo caso ainda não exista.
        /// </summary>
        private async Task<CarrinhoEntity> ObterOuCriarCarrinhoAsync(int clienteId)
        {
            var carrinho = await _context.Carrinhos
                .FirstOrDefaultAsync(c => c.ClienteId == clienteId);

            if (carrinho != null)
            {
                return carrinho;
            }

            carrinho = new CarrinhoEntity
            {
                ClienteId = clienteId,
                DataCriacao = DateTime.Now
            };

            _context.Carrinhos.Add(carrinho);
            await _context.SaveChangesAsync();

            return carrinho;
        }
        // Adicione esta propriedade para corrigir o erro CS1061
        public CarrinhoViewModel CarrinhoAtual { get; set; }

        // Exemplo de propriedade para o total dos produtos
        public decimal TotalProdutos
        {
            get
            {
                if (CarrinhoAtual?.Produtos == null)
                    return 0;
                decimal total = 0;
                foreach (var item in CarrinhoAtual.Produtos)
                {
                    total += item.Quantidade * item.PrecoNoMomento;
                }
                return total;
            }
        }

        public void OnGet()
        {
            // Inicialização de exemplo
            CarrinhoAtual = new CarrinhoViewModel
            {
                Produtos = new List<ItemCarrinhoViewModel>()
                // Preencha conforme necessário
            };
        }
    }

    // Exemplo de ViewModel para o carrinho
    public class CarrinhoViewModel
    {
        public List<ItemCarrinhoViewModel> Produtos { get; set; }
    }

    // Exemplo de ViewModel para um item do carrinho
    public class ItemCarrinhoViewModel
    {
        public int Id { get; set; }
        public ProdutoViewModel Produto { get; set; }
        public decimal PrecoNoMomento { get; set; }
        public int Quantidade { get; set; }
    }

    // Exemplo de ViewModel para produto
    public class ProdutoViewModel
    {
        public string Nome { get; set; }
    }
}