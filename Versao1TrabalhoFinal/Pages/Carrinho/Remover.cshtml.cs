using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;

namespace Versao1TrabalhoFinal.Pages.Carrinho
{
    /// <summary>
    /// Página responsável por remover um item do carrinho do cliente autenticado.
    /// </summary>
    [Authorize(Roles = "Cliente")]
    public class RemoverModel : PageModel
    {
        private readonly StandDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Inicializa uma nova instância da página de remoção de itens do carrinho.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        /// <param name="userManager">Gestor de utilizadores do Identity.</param>
        public RemoverModel(StandDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Remove um item do carrinho, consoante o tipo de identificador recebido.
        /// </summary>
        /// <param name="produtoId">Identificador do produto a remover.</param>
        /// <param name="servicoId">Identificador do serviço a remover.</param>
        /// <param name="veiculoStandId">Identificador do veículo do stand a remover.</param>
        /// <returns>Resultado da execução da página.</returns>
        public async Task<IActionResult> OnGetAsync(int? produtoId, int? servicoId, int? veiculoStandId)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(userId))
            {
                TempData["ErrorMessage"] = "Utilizador não autenticado.";
                return RedirectToPage("/Identity/Account/Login");
            }

            var cliente = await _context.Clientes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.IdentityUserId == userId);

            if (cliente == null)
            {
                TempData["ErrorMessage"] = "Cliente não encontrado para o utilizador autenticado.";
                return RedirectToPage("/Carrinho/Index");
            }

            var carrinho = await _context.Carrinhos
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ClienteId == cliente.Id);

            if (carrinho == null)
            {
                TempData["ErrorMessage"] = "Não foi encontrado um carrinho associado ao cliente.";
                return RedirectToPage("/Carrinho/Index");
            }

            var totalParametros = 0;

            if (produtoId.HasValue) totalParametros++;
            if (servicoId.HasValue) totalParametros++;
            if (veiculoStandId.HasValue) totalParametros++;

            if (totalParametros == 0)
            {
                TempData["ErrorMessage"] = "Não foi indicado nenhum item para remover.";
                return RedirectToPage("/Carrinho/Index");
            }

            if (totalParametros > 1)
            {
                TempData["ErrorMessage"] = "Foi indicado mais do que um tipo de item para remover. A operação é inválida.";
                return RedirectToPage("/Carrinho/Index");
            }

            if (produtoId.HasValue)
            {
                var itemProduto = await _context.CarrinhoProdutos
                    .FirstOrDefaultAsync(cp =>
                        cp.CarrinhoId == carrinho.Id &&
                        cp.ProdutoId == produtoId.Value);

                if (itemProduto == null)
                {
                    TempData["ErrorMessage"] = "O produto indicado não foi encontrado no carrinho.";
                    return RedirectToPage("/Carrinho/Index");
                }

                _context.CarrinhoProdutos.Remove(itemProduto);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Produto removido do carrinho com sucesso.";
                return Page();
            }

            if (servicoId.HasValue)
            {
                var itemServico = await _context.CarrinhoServicos
                    .FirstOrDefaultAsync(cs =>
                        cs.CarrinhoId == carrinho.Id &&
                        cs.ServicoId == servicoId.Value);

                if (itemServico == null)
                {
                    TempData["ErrorMessage"] = "O serviço indicado não foi encontrado no carrinho.";
                    return RedirectToPage("/Carrinho/Index");
                }

                _context.CarrinhoServicos.Remove(itemServico);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Serviço removido do carrinho com sucesso.";
                return Page();
            }

            if (veiculoStandId.HasValue)
            {
                var itemVeiculoStand = await _context.CarrinhoVeiculosStand
                    .FirstOrDefaultAsync(cvs =>
                        cvs.CarrinhoId == carrinho.Id &&
                        cvs.VeiculoStandId == veiculoStandId.Value);

                if (itemVeiculoStand == null)
                {
                    TempData["ErrorMessage"] = "O veículo do stand indicado não foi encontrado no carrinho.";
                    return RedirectToPage("/Carrinho/Index");
                }

                _context.CarrinhoVeiculosStand.Remove(itemVeiculoStand);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Veículo do stand removido do carrinho com sucesso.";
                return Page();
            }

            TempData["ErrorMessage"] = "Não foi possível processar a remoção do item.";
            return RedirectToPage("/Carrinho/Index");
        }
    }
}