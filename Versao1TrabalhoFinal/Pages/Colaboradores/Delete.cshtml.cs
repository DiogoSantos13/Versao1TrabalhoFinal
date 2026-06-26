using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Colaboradores
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly StandDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DeleteModel(
            StandDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Colaborador? Colaborador { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Colaborador = await _context.Colaboradores
                .FirstOrDefaultAsync(c => c.Id == id);

            if (Colaborador == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var colaborador = await _context.Colaboradores
                .FirstOrDefaultAsync(c => c.Id == id);

            if (colaborador == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(colaborador.IdentityUserId);

            _context.Colaboradores.Remove(colaborador);
            await _context.SaveChangesAsync();

            if (user != null)
            {
                var deleteResult = await _userManager.DeleteAsync(user);

                if (!deleteResult.Succeeded)
                {
                    TempData["ErrorMessage"] = string.Join(" | ", deleteResult.Errors.Select(e => e.Description));
                    return RedirectToPage("./Index");
                }
            }

            TempData["SuccessMessage"] = "Colaborador eliminado com sucesso.";
            return RedirectToPage("./Index");
        }
    }
}