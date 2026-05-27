using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Produtos
{
    /// <summary>
    /// Página responsável pela criação de produtos para venda.
    /// </summary>
    [Authorize(Roles = "Admin,Colaborador")]
    public class CreateModel : PageModel
    {
        private readonly StandDbContext _context;
        private readonly IWebHostEnvironment _environment;

        /// <summary>
        /// Inicializa uma nova instância da página de criação de produtos.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        /// <param name="environment">Ambiente da aplicação para aceder ao wwwroot.</param>
        public CreateModel(StandDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        /// <summary>
        /// Produto em criação.
        /// </summary>
        [BindProperty]
        public Produto Produto { get; set; } = new();

        /// <summary>
        /// Ficheiros de imagem enviados no formulário.
        /// </summary>
        [BindProperty]
        public List<IFormFile> UploadImagens { get; set; } = new();

        /// <summary>
        /// Lista de fornecedores disponíveis para o produto.
        /// </summary>
        public SelectList FornecedoresOptions { get; set; } = default!;

        /// <summary>
        /// Carrega a página de criação e os fornecedores disponíveis.
        /// </summary>
        /// <returns>Página de criação.</returns>
        public async Task<IActionResult> OnGetAsync()
        {
            await CarregarFornecedoresAsync();
            return Page();
        }

        /// <summary>
        /// Processa a submissão do formulário de criação do produto.
        /// </summary>
        /// <returns>Redireciona para a listagem em caso de sucesso.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            var fornecedorValido = await _context.Fornecedores
                .AsNoTracking()
                .AnyAsync(f => f.Id == Produto.FornecedorId);

            if (!fornecedorValido)
            {
                ModelState.AddModelError("Produto.FornecedorId", "Selecione um fornecedor válido.");
            }

            if (!ModelState.IsValid)
            {
                await CarregarFornecedoresAsync();
                return Page();
            }

            _context.Produtos.Add(Produto);
            await _context.SaveChangesAsync();

            if (UploadImagens != null && UploadImagens.Count > 0)
            {
                string pastaRelativa = Path.Combine("images", "produtos");
                string pastaFisica = Path.Combine(_environment.WebRootPath, pastaRelativa);

                if (!Directory.Exists(pastaFisica))
                {
                    Directory.CreateDirectory(pastaFisica);
                }

                int ordem = 0;

                foreach (var ficheiro in UploadImagens)
                {
                    if (ficheiro == null || ficheiro.Length == 0)
                    {
                        continue;
                    }

                    string extensao = Path.GetExtension(ficheiro.FileName);
                    string nomeFicheiro = $"{Guid.NewGuid()}{extensao}";
                    string caminhoFisico = Path.Combine(pastaFisica, nomeFicheiro);

                    using (var stream = new FileStream(caminhoFisico, FileMode.Create))
                    {
                        await ficheiro.CopyToAsync(stream);
                    }

                    var imagem = new ImagemEntidade
                    {
                        Url = $"/images/produtos/{nomeFicheiro}",
                        Alt = Produto.Nome,
                        Ordem = ordem,
                        Principal = ordem == 0,
                        EntidadeId = Produto.Id,
                        TipoEntidade = "Produto"
                    };

                    _context.ImagensEntidade.Add(imagem);
                    ordem++;
                }

                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Produto criado com sucesso.";
            return RedirectToPage("./Index");
        }

        /// <summary>
        /// Carrega a lista de fornecedores para o dropdown.
        /// </summary>
        private async Task CarregarFornecedoresAsync()
        {
            var fornecedores = await _context.Fornecedores
                .AsNoTracking()
                .OrderBy(f => f.Nome)
                .Select(f => new { f.Id, f.Nome })
                .ToListAsync();

            FornecedoresOptions = new SelectList(fornecedores, "Id", "Nome", Produto.FornecedorId);
        }
    }
}