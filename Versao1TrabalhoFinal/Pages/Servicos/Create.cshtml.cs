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
        /// URLs da galeria, uma por linha.
        /// </summary>
        [BindProperty]
        public string? GaleriaUrls { get; set; }

        /// <summary>
        /// Carrega a página de criação.
        /// </summary>
        public void OnGet()
        {
            Servico.Ativo = true;
        }

        /// <summary>
        /// Cria um novo serviço e, se existirem, guarda também as imagens da galeria.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            var linksGaleria = ObterLinksValidos(GaleriaUrls);

            if (!string.IsNullOrWhiteSpace(GaleriaUrls) && linksGaleria.Count == 0)
            {
                ModelState.AddModelError("GaleriaUrls", "Introduza pelo menos um URL válido, um por linha.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Servicos.Add(Servico);
            await _context.SaveChangesAsync();

            if (linksGaleria.Count > 0)
            {
                for (int i = 0; i < linksGaleria.Count; i++)
                {
                    _context.ImagensEntidade.Add(new ImagemEntidade
                    {
                        Url = linksGaleria[i],
                        Alt = string.IsNullOrWhiteSpace(Servico.Nome) ? "Imagem do serviço" : Servico.Nome,
                        Ordem = i,
                        Principal = i == 0,
                        EntidadeId = Servico.Id,
                        TipoEntidade = "Servico"
                    });
                }

                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Serviço criado com sucesso.";
            return RedirectToPage("/Servicos/Index");
        }

        /// <summary>
        /// Obtém links válidos a partir do texto introduzido.
        /// </summary>
        private static List<string> ObterLinksValidos(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return new List<string>();
            }

            return texto
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Where(l => Uri.TryCreate(l, UriKind.Absolute, out var uri) &&
                            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                .Distinct()
                .ToList();
        }
    }
}