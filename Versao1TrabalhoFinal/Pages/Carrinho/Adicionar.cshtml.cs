using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using CarrinhoEntity = Versao1TrabalhoFinal.Models.Carrinho;
using CarrinhoItemEntity = Versao1TrabalhoFinal.Models.CarrinhoItem;
using EstadoVeiculoStandEntity = Versao1TrabalhoFinal.Models.EstadoVeiculoStand;

namespace Versao1TrabalhoFinal.Pages.Carrinho
{
    /// <summary>
    /// Página responsável por adicionar um veículo do stand ao carrinho do cliente autenticado.
    /// </summary>
    [Authorize(Roles = "Cliente")]
    public class AdicionarModel : PageModel
    {
        private readonly StandDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Inicializa uma nova instância da página de adição ao carrinho.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        /// <param name="userManager">Gestor de utilizadores do Identity.</param>
        public AdicionarModel(StandDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Adiciona um veículo do stand ao carrinho do cliente autenticado.
        /// </summary>
        /// <param name="veiculoStandId">Identificador do veículo do stand.</param>
        /// <returns>Redireciona para a página do carrinho ou para a listagem do stand.</returns>
        public async Task<IActionResult> OnGetAsync(int veiculoStandId)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(userId))
            {
                TempData["ErrorMessage"] = "Utilizador não autenticado.";
                return RedirectToPage("/Account/Login");
            }

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.IdentityUserId == userId);

            if (cliente == null)
            {
                TempData["ErrorMessage"] = $"Cliente não encontrado para o utilizador autenticado. UserId: {userId}";
                return RedirectToPage("/VeiculosStand/Index");
            }

            var veiculoStand = await _context.VeiculosStand
                .AsNoTracking()
                .FirstOrDefaultAsync(vs => vs.Id == veiculoStandId);

            if (veiculoStand == null)
            {
                TempData["ErrorMessage"] = "O veículo pretendido não foi encontrado.";
                return RedirectToPage("/VeiculosStand/Index");
            }

            if (!string.Equals(veiculoStand.Estado, EstadoVeiculoStandEntity.Disponivel, StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = $"Este veículo já não se encontra disponível. Estado atual: {veiculoStand.Estado}";
                return RedirectToPage("/VeiculosStand/Index");
            }

            var carrinho = await _context.Carrinhos
                .FirstOrDefaultAsync(c => c.ClienteId == cliente.Id);

            if (carrinho == null)
            {
                carrinho = new CarrinhoEntity
                {
                    ClienteId = cliente.Id,
                    DataCriacao = DateTime.Now
                };

                _context.Carrinhos.Add(carrinho);
                await _context.SaveChangesAsync();
            }

            var itemExistente = await _context.CarrinhoItens
                .AnyAsync(ci => ci.CarrinhoId == carrinho.Id && ci.VeiculoStandId == veiculoStand.Id);

            if (itemExistente)
            {
                TempData["SuccessMessage"] = "O veículo já se encontra no carrinho.";
                return RedirectToPage("/Carrinho/Index");
            }

            var item = new CarrinhoItemEntity
            {
                CarrinhoId = carrinho.Id,
                VeiculoStandId = veiculoStand.Id,
                PrecoNoMomento = veiculoStand.Preco,
                DataAdicao = DateTime.Now
            };

            _context.CarrinhoItens.Add(item);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.InnerException?.Message ?? ex.Message;
                return RedirectToPage("/VeiculosStand/Index");
            }

            TempData["SuccessMessage"] = "Veículo adicionado ao carrinho com sucesso.";
            return RedirectToPage("/Carrinho/Index");
        }
    }
}
