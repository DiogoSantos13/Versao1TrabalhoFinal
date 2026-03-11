using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Servicos
{
    [Authorize(Roles = "Colaborador,Admin")]
    public class CreateModel : PageModel
    {
        private readonly StandDbContext _context;

        public CreateModel(StandDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Servico Servico { get; set; } = new();

        public void OnGet()
        {
            Servico.Ativo = true;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.Servicos.Add(Servico);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Servicos/Index");
        }
    }
}
