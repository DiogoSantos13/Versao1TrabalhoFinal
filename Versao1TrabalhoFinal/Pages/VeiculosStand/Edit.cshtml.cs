using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.VeiculosStand
{
    /// <summary>
    /// Página responsável pela edição de veículos do stand.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly StandDbContext _context;

        public EditModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Registo do veículo do stand.
        /// </summary>
        [BindProperty]
        public VeiculoStand VeiculoStand { get; set; } = new();

        /// <summary>
        /// Veículo associado ao registo do stand.
        /// </summary>
        [BindProperty]
        public Veiculo Veiculo { get; set; } = new();

        /// <summary>
        /// Novos links para adicionar à galeria, um por linha.
        /// </summary>
        [BindProperty]
        public string? NovasGaleriaUrls { get; set; }

        /// <summary>
        /// Carrega os dados do veículo do stand e respetivo veículo.
        /// </summary>
        /// <param name="id">Id do veículo do stand.</param>
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var item = await _context.VeiculosStand
                .Include(vs => vs.Veiculo)
                .AsNoTracking()
                .FirstOrDefaultAsync(vs => vs.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            VeiculoStand = item;
            Veiculo = item.Veiculo ?? new Veiculo();

            VeiculoStand.Imagens = await _context.ImagensEntidade
                .AsNoTracking()
                .Where(i => i.TipoEntidade == "VeiculoStand" && i.EntidadeId == VeiculoStand.Id)
                .OrderByDescending(i => i.Principal)
                .ThenBy(i => i.Ordem)
                .ToListAsync();

            return Page();
        }

        /// <summary>
        /// Guarda alterações do veículo do stand e do veículo associado.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            var existente = await _context.VeiculosStand
                .Include(vs => vs.Veiculo)
                .FirstOrDefaultAsync(vs => vs.Id == VeiculoStand.Id);

            if (existente == null)
            {
                return NotFound();
            }

            if (existente.Veiculo == null)
            {
                ModelState.AddModelError(string.Empty, "O veículo associado não foi encontrado.");
            }

            if (!ModelState.IsValid)
            {
                await RecarregarImagensAsync(VeiculoStand.Id);
                return Page();
            }

            existente.Preco = VeiculoStand.Preco;
            existente.Estado = VeiculoStand.Estado;
            existente.Quilometros = VeiculoStand.Quilometros;
            existente.Descricao = VeiculoStand.Descricao;
            existente.DataEntrada = VeiculoStand.DataEntrada;

            if (existente.Veiculo != null)
            {
                existente.Veiculo.Marca = Veiculo.Marca;
                existente.Veiculo.Modelo = Veiculo.Modelo;
                existente.Veiculo.Ano = Veiculo.Ano;
                existente.Veiculo.Tipo = Veiculo.Tipo;
                existente.Veiculo.Cilindrada = Veiculo.Cilindrada;
                existente.Veiculo.Combustivel = Veiculo.Combustivel;
                existente.Veiculo.Matricula = Veiculo.Matricula;
                existente.Veiculo.VIN = Veiculo.VIN;
                existente.Veiculo.quilometragem = Veiculo.quilometragem;
            }

            var novosLinks = ObterLinksValidos(NovasGaleriaUrls);

            if (!string.IsNullOrWhiteSpace(NovasGaleriaUrls) && novosLinks.Count == 0)
            {
                ModelState.AddModelError("NovasGaleriaUrls", "Introduza pelo menos um URL válido, um por linha.");
                await RecarregarImagensAsync(VeiculoStand.Id);
                return Page();
            }

            await _context.SaveChangesAsync();

            if (novosLinks.Count > 0)
            {
                var imagensExistentes = await _context.ImagensEntidade
                    .Where(i => i.TipoEntidade == "VeiculoStand" && i.EntidadeId == existente.Id)
                    .OrderBy(i => i.Ordem)
                    .ToListAsync();

                int proximaOrdem = imagensExistentes.Any() ? imagensExistentes.Max(i => i.Ordem) + 1 : 0;
                bool jaExistePrincipal = imagensExistentes.Any(i => i.Principal);

                for (int i = 0; i < novosLinks.Count; i++)
                {
                    _context.ImagensEntidade.Add(new ImagemEntidade
                    {
                        Url = novosLinks[i],
                        Alt = $"Imagem de {existente.Veiculo?.Marca} {existente.Veiculo?.Modelo}".Trim(),
                        Ordem = proximaOrdem + i,
                        Principal = !jaExistePrincipal && i == 0,
                        EntidadeId = existente.Id,
                        TipoEntidade = "VeiculoStand"
                    });
                }

                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Veículo do stand atualizado com sucesso.";
            return RedirectToPage("./Index");
        }

        /// <summary>
        /// Remove uma imagem da galeria.
        /// </summary>
        public async Task<IActionResult> OnPostDeleteImagemAsync(int id, int imagemId)
        {
            var item = await _context.VeiculosStand
                .AsNoTracking()
                .FirstOrDefaultAsync(vs => vs.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            var imagem = await _context.ImagensEntidade
                .FirstOrDefaultAsync(i => i.Id == imagemId && i.TipoEntidade == "VeiculoStand" && i.EntidadeId == id);

            if (imagem == null)
            {
                TempData["ErrorMessage"] = "Imagem não encontrada.";
                return RedirectToPage(new { id });
            }

            bool eraPrincipal = imagem.Principal;

            _context.ImagensEntidade.Remove(imagem);
            await _context.SaveChangesAsync();

            if (eraPrincipal)
            {
                var primeira = await _context.ImagensEntidade
                    .Where(i => i.TipoEntidade == "VeiculoStand" && i.EntidadeId == id)
                    .OrderBy(i => i.Ordem)
                    .FirstOrDefaultAsync();

                if (primeira != null)
                {
                    primeira.Principal = true;
                    await _context.SaveChangesAsync();
                }
            }

            TempData["SuccessMessage"] = "Imagem removida com sucesso.";
            return RedirectToPage(new { id });
        }

        /// <summary>
        /// Define uma imagem como principal.
        /// </summary>
        public async Task<IActionResult> OnPostDefinirPrincipalAsync(int id, int imagemId)
        {
            var item = await _context.VeiculosStand
                .AsNoTracking()
                .FirstOrDefaultAsync(vs => vs.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            var imagens = await _context.ImagensEntidade
                .Where(i => i.TipoEntidade == "VeiculoStand" && i.EntidadeId == id)
                .ToListAsync();

            if (!imagens.Any())
            {
                TempData["ErrorMessage"] = "Não existem imagens para atualizar.";
                return RedirectToPage(new { id });
            }

            foreach (var imagem in imagens)
            {
                imagem.Principal = imagem.Id == imagemId;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Imagem principal atualizada com sucesso.";
            return RedirectToPage(new { id });
        }

        /// <summary>
        /// Recarrega as imagens para voltar a mostrar a página em caso de erro.
        /// </summary>
        private async Task RecarregarImagensAsync(int veiculoStandId)
        {
            VeiculoStand.Imagens = await _context.ImagensEntidade
                .AsNoTracking()
                .Where(i => i.TipoEntidade == "VeiculoStand" && i.EntidadeId == veiculoStandId)
                .OrderByDescending(i => i.Principal)
                .ThenBy(i => i.Ordem)
                .ToListAsync();
        }

        /// <summary>
        /// Converte o texto multi-linha numa lista de URLs válidos.
        /// </summary>
        private static List<string> ObterLinksValidos(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return new List<string>();
            }

            var linhas = texto
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Distinct()
                .ToList();

            var linksValidos = new List<string>();

            foreach (var linha in linhas)
            {
                if (Uri.TryCreate(linha, UriKind.Absolute, out var uri) &&
                    (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                {
                    linksValidos.Add(linha);
                }
            }

            return linksValidos;
        }
    }
}