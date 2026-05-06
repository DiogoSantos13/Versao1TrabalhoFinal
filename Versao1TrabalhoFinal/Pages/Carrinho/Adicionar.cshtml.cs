using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;
using CarrinhoEntity = Versao1TrabalhoFinal.Models.Carrinho;
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
        /// Veículo do stand carregado para apresentação na página.
        /// </summary>
        public VeiculoStand? VeiculoStand { get; set; }

        /// <summary>
        /// Processa o pedido de adição de um veículo do stand ao carrinho.
        /// </summary>
        /// <param name="veiculoStandId">Identificador do veículo do stand.</param>
        /// <returns>Resultado da execução da página.</returns>
        public async Task<IActionResult> OnGetAsync(int veiculoStandId)
        {
            if (veiculoStandId <= 0)
            {
                TempData["ErrorMessage"] = "O identificador do veículo é inválido.";
                return RedirectToPage("/VeiculosStand/Index");
            }

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
                return RedirectToPage("/VeiculosStand/Index");
            }

            VeiculoStand = await _context.VeiculosStand
                .Include(vs => vs.Veiculo)
                .FirstOrDefaultAsync(vs => vs.Id == veiculoStandId);

            if (VeiculoStand == null)
            {
                TempData["ErrorMessage"] = "O veículo pretendido não foi encontrado.";
                return RedirectToPage("/VeiculosStand/Index");
            }

            if (!string.Equals(VeiculoStand.Estado, EstadoVeiculoStandEntity.Disponivel, StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = $"Este veículo já não se encontra disponível. Estado atual: {VeiculoStand.Estado}";
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

            var itemExistente = await _context.CarrinhoVeiculosStand
                .AsNoTracking()
                .FirstOrDefaultAsync(cvs =>
                    cvs.CarrinhoId == carrinho.Id &&
                    cvs.VeiculoStandId == VeiculoStand.Id);

            if (itemExistente != null)
            {
                TempData["ErrorMessage"] = "Este veículo já se encontra no carrinho.";
                return RedirectToPage("/Carrinho/Index");
            }

            var item = new CarrinhoVeiculoStand
            {
                CarrinhoId = carrinho.Id,
                VeiculoStandId = VeiculoStand.Id,
                PrecoNoMomento = VeiculoStand.Preco,
                DataAdicao = DateTime.Now
            };

            _context.CarrinhoVeiculosStand.Add(item);

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
            return Page();
        }
    }
}