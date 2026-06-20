using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;
using ClienteEntity = Versao1TrabalhoFinal.Models.Cliente;

namespace Versao1TrabalhoFinal.Pages.Clientes
{
    /// <summary>
    /// Página responsável pela confirmação e eliminação de clientes.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        /// <summary>
        /// Contexto da base de dados da aplicação.
        /// </summary>
        private readonly StandDbContext _context;

        /// <summary>
        /// Serviço de gestão de utilizadores do ASP.NET Core Identity.
        /// </summary>
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Inicializa uma nova instância da página de eliminação de clientes.
        /// </summary>
        /// <param name="context">Contexto da base de dados da aplicação.</param>
        /// <param name="userManager">Serviço de gestão de utilizadores do ASP.NET Core Identity.</param>
        public DeleteModel(StandDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Cliente apresentado na página de confirmação.
        /// </summary>
        [BindProperty]
        public ClienteEntity Cliente { get; set; } = default!;

        /// <summary>
        /// Carrega os dados do cliente para confirmação de eliminação.
        /// </summary>
        /// <param name="id">Identificador do cliente.</param>
        /// <returns>Resultado da execução da página.</returns>
        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Procura o cliente pelo identificador.
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);

            // Se o cliente não existir, devolve 404.
            if (cliente == null)
            {
                return NotFound();
            }

            // Coloca o cliente na propriedade usada pela página.
            Cliente = cliente;
            return Page();
        }

        /// <summary>
        /// Processa a eliminação do cliente, bloqueando a operação caso existam veículos associados.
        /// </summary>
        /// <param name="id">Identificador do cliente.</param>
        /// <returns>Redireciona para a listagem com mensagem de sucesso ou erro.</returns>
        public async Task<IActionResult> OnPostAsync(int id)
        {
            // Procura o cliente pelo identificador.
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);

            // Se o cliente não existir, devolve 404.
            if (cliente == null)
            {
                return NotFound();
            }

            // Verifica se o cliente tem veículos associados.
            var temVeiculos = await _context.Veiculos.AnyAsync(v => v.ClienteId == id);

            // Se existirem veículos associados, não permite eliminar.
            if (temVeiculos)
            {
                TempData["ErrorMessage"] = "Não é possível eliminar este cliente porque existem veículos associados.";
                return RedirectToPage("./Index");
            }

            // Tenta obter o utilizador Identity associado ao cliente.
            IdentityUser? user = null;

            if (!string.IsNullOrWhiteSpace(cliente.IdentityUserId))
            {
                user = await _userManager.FindByIdAsync(cliente.IdentityUserId);
            }

            // Remove o cliente da base de dados.
            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            // Se existir utilizador Identity associado, remove-o também.
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            TempData["SuccessMessage"] = "Cliente eliminado com sucesso.";
            return RedirectToPage("./Index");
        }
    }
}