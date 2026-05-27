using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.VeiculosStand
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly StandDbContext _context;

        public CreateModel(StandDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public VeiculoStand VeiculoStand { get; set; } = new();

        [BindProperty]
        public bool CriarNovoVeiculo { get; set; }

        [BindProperty]
        public Veiculo NovoVeiculo { get; set; } = new();

        [BindProperty]
        public string? GaleriaUrls { get; set; }

        public SelectList VeiculosSelect { get; set; } = default!;

        public async Task OnGetAsync()
        {
            VeiculoStand.DataEntrada = DateTime.Now;
            await LoadVeiculosAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadVeiculosAsync();

            try
            {
                PrepararModelState();

                if (CriarNovoVeiculo)
                {
                    ValidarNovoVeiculo();
                }
                else
                {
                    await ValidarVeiculoExistenteAsync();
                }

                ValidarGaleria();

                if (!ModelState.IsValid)
                {
                    return Page();
                }

                if (CriarNovoVeiculo)
                {
                    NovoVeiculo.ClienteId = null;
                    NovoVeiculo.Cliente = null;

                    if (string.IsNullOrWhiteSpace(NovoVeiculo.ImagemUrl))
                    {
                        NovoVeiculo.ImagemUrl = null;
                    }

                    _context.Veiculos.Add(NovoVeiculo);
                    await _context.SaveChangesAsync();

                    VeiculoStand.VeiculoId = NovoVeiculo.Id;
                }

                _context.VeiculosStand.Add(VeiculoStand);
                await _context.SaveChangesAsync();

                var links = ObterLinksValidos(GaleriaUrls);

                if (links.Count > 0)
                {
                    for (int i = 0; i < links.Count; i++)
                    {
                        _context.ImagensEntidade.Add(new ImagemEntidade
                        {
                            Url = links[i],
                            Alt = CriarNovoVeiculo
                                ? $"{NovoVeiculo.Marca} {NovoVeiculo.Modelo}".Trim()
                                : "Imagem do veículo do stand",
                            Ordem = i,
                            Principal = i == 0,
                            EntidadeId = VeiculoStand.Id,
                            TipoEntidade = "VeiculoStand"
                        });
                    }

                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = "Veículo do stand criado com sucesso.";
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException ex)
            {
                var mensagem = ex.InnerException?.Message ?? ex.Message;

                if (mensagem.Contains("FK__Veiculos__Client", StringComparison.OrdinalIgnoreCase) ||
                    mensagem.Contains("FOREIGN KEY constraint", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(string.Empty,
                        "Erro ao criar o veículo. O ClienteId do veículo do stand tem de ficar vazio.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty,
                        "Ocorreu um erro ao guardar os dados na base de dados.");
                }

                return Page();
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty,
                    "Ocorreu um erro inesperado ao criar o veículo do stand.");
                return Page();
            }
        }

        private void PrepararModelState()
        {
            if (CriarNovoVeiculo)
            {
                ModelState.Remove("VeiculoStand.VeiculoId");
            }
            else
            {
                ModelState.Remove("NovoVeiculo.Marca");
                ModelState.Remove("NovoVeiculo.Modelo");
                ModelState.Remove("NovoVeiculo.Ano");
                ModelState.Remove("NovoVeiculo.Tipo");
                ModelState.Remove("NovoVeiculo.Cilindrada");
                ModelState.Remove("NovoVeiculo.Combustivel");
                ModelState.Remove("NovoVeiculo.Matricula");
                ModelState.Remove("NovoVeiculo.VIN");
                ModelState.Remove("NovoVeiculo.Cor");
                ModelState.Remove("NovoVeiculo.Quilometragem");
                ModelState.Remove("NovoVeiculo.ClienteId");
                ModelState.Remove("NovoVeiculo.Cliente");
                ModelState.Remove("NovoVeiculo.ImagemUrl");
            }
        }

        private async Task ValidarVeiculoExistenteAsync()
        {
            if (VeiculoStand.VeiculoId <= 0)
            {
                ModelState.AddModelError("VeiculoStand.VeiculoId", "Selecione um veículo.");
                return;
            }

            var veiculoExiste = await _context.Veiculos
                .AsNoTracking()
                .AnyAsync(v => v.Id == VeiculoStand.VeiculoId);

            if (!veiculoExiste)
            {
                ModelState.AddModelError("VeiculoStand.VeiculoId", "Selecione um veículo válido.");
            }
        }

        private void ValidarNovoVeiculo()
        {
            if (string.IsNullOrWhiteSpace(NovoVeiculo.Marca))
            {
                ModelState.AddModelError("NovoVeiculo.Marca", "Indique a marca.");
            }

            if (string.IsNullOrWhiteSpace(NovoVeiculo.Modelo))
            {
                ModelState.AddModelError("NovoVeiculo.Modelo", "Indique o modelo.");
            }

            if (NovoVeiculo.Ano <= 0)
            {
                ModelState.AddModelError("NovoVeiculo.Ano", "Indique um ano válido.");
            }

            if (string.IsNullOrWhiteSpace(NovoVeiculo.Matricula))
            {
                ModelState.AddModelError("NovoVeiculo.Matricula", "Indique a matrícula.");
            }

            if (NovoVeiculo.quilometragem < 0)
            {
                ModelState.AddModelError("NovoVeiculo.quilometragem", "Indique uma quilometragem válida.");
            }
        }

        private void ValidarGaleria()
        {
            var links = ObterLinksValidos(GaleriaUrls);

            if (!string.IsNullOrWhiteSpace(GaleriaUrls) && links.Count == 0)
            {
                ModelState.AddModelError("GaleriaUrls",
                    "Introduza pelo menos um URL válido, um por linha.");
            }
        }

        private async Task LoadVeiculosAsync()
        {
            var veiculos = await _context.Veiculos
                .AsNoTracking()
                .OrderBy(v => v.Marca)
                .ThenBy(v => v.Modelo)
                .Select(v => new
                {
                    v.Id,
                    NomeCompleto = v.Marca + " " + v.Modelo
                })
                .ToListAsync();

            VeiculosSelect = new SelectList(veiculos, "Id", "NomeCompleto");
        }

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