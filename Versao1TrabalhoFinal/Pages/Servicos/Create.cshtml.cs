using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Servicos
{
    /// <summary>
    /// Página responsável pela criação de serviços.
    /// </summary>
    [Authorize(Roles = "Colaborador,Admin")]
    public class CreateModel : PageModel
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Inicializa uma nova instância da página de criação de serviços.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public CreateModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Serviço a criar.
        /// </summary>
        [BindProperty]
        public Servico Servico { get; set; } = new();

        /// <summary>
        /// Carrega a página de criação.
        /// </summary>
        public void OnGet()
        {
        }

        /// <summary>
        /// Cria um novo serviço.
        /// </summary>
        /// <returns>Redireciona para a listagem ou volta à página em caso de erro.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Servicos.Add(Servico);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Servicos/Index");
        }
    }
}
