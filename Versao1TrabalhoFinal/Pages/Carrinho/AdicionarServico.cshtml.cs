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
        /// Inicializa uma nova instância da página de adição de serviços ao carrinho.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        /// <param name="userManager">Gestor de utilizadores do Identity.</param>
        public AdicionarServicoModel(StandDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Adiciona um serviço ao carrinho do cliente autenticado.
        /// </summary>
        /// <param name="servicoId">Identificador do serviço.</param>
        /// <returns>Redireciona para a página do carrinho ou para a listagem de serviços.</returns>
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
                TempData["ErrorMessage"] = "O serviço selecionado está inativo.";
                return RedirectToPage("/Servicos/Index");
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

            var itemExistente = await _context.CarrinhoServicos
                .AnyAsync(cs => cs.CarrinhoId == carrinho.Id && cs.ServicoId == servico.Id);

            if (itemExistente)
            {
                TempData["SuccessMessage"] = "O serviço já se encontra no carrinho.";
                return RedirectToPage("/Carrinho/Index");
            }

            var itemServico = new CarrinhoServicoEntity
            {
                CarrinhoId = carrinho.Id,
                ServicoId = servico.Id,
                PrecoNoMomento = servico.PrecoBase,
                DataAdicao = DateTime.Now
            };

            _context.CarrinhoServicos.Add(itemServico);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Serviço adicionado ao carrinho com sucesso.";
            return RedirectToPage("/Carrinho/Index");
        }

        /// <summary>
        /// Obtém o cliente autenticado a partir do utilizador atual.
        /// </summary>
        /// <returns>Cliente autenticado ou nulo.</returns>
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
    }
}
