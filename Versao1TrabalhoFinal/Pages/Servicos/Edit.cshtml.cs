using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Servicos
{
    /// <summary>
    /// Página responsável pela edição de serviços.
    /// </summary>
    [Authorize(Roles = "Colaborador,Admin")]
    public class EditModel : PageModel
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Inicializa uma nova instância da página de edição de serviços.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public EditModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Serviço a editar.
        /// </summary>
        [BindProperty]
        public Servico Servico { get; set; } = new();

        /// <summary>
        /// URLs da galeria, uma por linha.
        /// </summary>
        [BindProperty]
        public string? GaleriaUrls { get; set; }

        /// <summary>
        /// Carrega o serviço a editar.
        /// </summary>
        /// <param name="id">Identificador do serviço.</param>
        /// <returns>A página ou NotFound.</returns>
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var servico = await _context.Servicos
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (servico == null)
            {
                return NotFound();
            }

            Servico = servico;

            var imagens = await _context.ImagensEntidade
                .AsNoTracking()
                .Where(i => i.TipoEntidade == "Servico" && i.EntidadeId == id)
                .OrderByDescending(i => i.Principal)
                .ThenBy(i => i.Ordem)
                .Select(i => i.Url)
                .ToListAsync();

            GaleriaUrls = string.Join(Environment.NewLine, imagens);

            return Page();
        }

        /// <summary>
        /// Guarda as alterações do serviço e atualiza a galeria.
        /// </summary>
        /// <returns>Redireciona para a listagem ou volta à página em caso de erro.</returns>
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

            var servicoDb = await _context.Servicos
                .FirstOrDefaultAsync(s => s.Id == Servico.Id);

            if (servicoDb == null)
            {
                return NotFound();
            }

            servicoDb.Nome = Servico.Nome;
            servicoDb.Descricao = Servico.Descricao;
            servicoDb.PrecoBase = Servico.PrecoBase;
            servicoDb.TempoEstimado = Servico.TempoEstimado;
            servicoDb.ImagemUrl = string.IsNullOrWhiteSpace(Servico.ImagemUrl) ? null : Servico.ImagemUrl;
            servicoDb.Ativo = Servico.Ativo;

            var imagensExistentes = await _context.ImagensEntidade
                .Where(i => i.TipoEntidade == "Servico" && i.EntidadeId == Servico.Id)
                .ToListAsync();

            if (imagensExistentes.Any())
            {
                _context.ImagensEntidade.RemoveRange(imagensExistentes);
            }

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

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var existe = await _context.Servicos
                    .AsNoTracking()
                    .AnyAsync(s => s.Id == Servico.Id);

                if (!existe)
                {
                    return NotFound();
                }

                throw;
            }

            TempData["SuccessMessage"] = "Serviço atualizado com sucesso.";
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