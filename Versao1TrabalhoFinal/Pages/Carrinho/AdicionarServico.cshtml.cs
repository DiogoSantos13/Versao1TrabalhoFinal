using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using CarrinhoEntity = Versao1TrabalhoFinal.Models.Carrinho;
using CarrinhoServicoEntity = Versao1TrabalhoFinal.Models.CarrinhoServico;
using ClienteEntity = Versao1TrabalhoFinal.Models.Cliente;

namespace Versao1TrabalhoFinal.Pages.Carrinho
{
    /// <summary>
    /// Página responsável por adicionar um serviço ao carrinho do cliente autenticado.
    /// </summary>
    [Authorize(Roles = "Cliente")]
    public class AdicionarServicoModel : PageModel
    {
        private readonly StandDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Inicializa uma nova instância da página de adição de serviço ao carrinho.
        /// </summary>
        public AdicionarServicoModel(StandDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Adiciona o serviço ao carrinho do cliente autenticado.
        /// </summary>
        /// <param name="servicoId">Identificador do serviço.</param>
        public async Task<IActionResult> OnGetAsync(int servicoId)
        {
            var cliente = await ObterClienteAutenticadoAsync();

            if (cliente == null)
            {
                TempData["ErrorMessage"] = "Cliente autenticado não encontrado.";
                return RedirectToPage("/Servicos/Index");
            }

            var servico = await _context.Servicos
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == servicoId);

            if (servico == null)
            {
                TempData["ErrorMessage"] = "Serviço não encontrado.";
                return RedirectToPage("/Servicos/Index");
            }

            if (!servico.Ativo)
            {
                TempData["ErrorMessage"] = "O serviço selecionado não está ativo.";
                return RedirectToPage("/Servicos/Index");
            }

            var carrinho = await ObterOuCriarCarrinhoAsync(cliente.Id);

            var itemExistente = await _context.CarrinhoServicos
                .FirstOrDefaultAsync(cs => cs.CarrinhoId == carrinho.Id && cs.ServicoId == servico.Id);

            if (itemExistente != null)
            {
                TempData["SuccessMessage"] = "O serviço já se encontra no carrinho.";
                return RedirectToPage("/Carrinho/Index");
            }

            var item = new CarrinhoServicoEntity
            {
                CarrinhoId = carrinho.Id,
                ServicoId = servico.Id,
                PrecoNoMomento = servico.PrecoBase,
                DataAdicao = DateTime.Now
            };

            _context.CarrinhoServicos.Add(item);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Serviço adicionado ao carrinho com sucesso.";
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
    }
}