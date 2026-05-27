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
    /// Página responsável pela edição de produtos.
    /// </summary>
    [Authorize(Roles = "Admin,Colaborador")]
    public class EditModel : PageModel
    {
        private readonly StandDbContext _context;
        private readonly IWebHostEnvironment _environment;

        /// <summary>
        /// Inicializa uma nova instância da página de edição de produtos.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        /// <param name="environment">Ambiente da aplicação.</param>
        public EditModel(StandDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        /// <summary>
        /// Produto que está a ser editado.
        /// </summary>
        [BindProperty]
        public Produto Produto { get; set; } = new();

        /// <summary>
        /// Novas imagens enviadas para o produto.
        /// </summary>
        [BindProperty]
        public List<IFormFile> UploadImagens { get; set; } = new();

        /// <summary>
        /// Lista de fornecedores disponíveis para o dropdown.
        /// </summary>
        public SelectList FornecedoresSelect { get; set; } = default!;

        /// <summary>
        /// Carrega o produto, imagens e fornecedores para edição.
        /// </summary>
        /// <param name="id">Identificador do produto.</param>
        /// <returns>Página de edição ou NotFound.</returns>
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var produto = await _context.Produtos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (produto == null)
            {
                return NotFound();
            }

            produto.Imagens = await _context.ImagensEntidade
                .AsNoTracking()
                .Where(i => i.TipoEntidade == "Produto" && i.EntidadeId == produto.Id)
                .OrderByDescending(i => i.Principal)
                .ThenBy(i => i.Ordem)
                .ToListAsync();

            Produto = produto;
            await CarregarFornecedoresAsync(Produto.FornecedorId);

            return Page();
        }

        /// <summary>
        /// Guarda as alterações efetuadas ao produto e adiciona novas imagens, se existirem.
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
                Produto.Imagens = await ObterImagensProdutoAsync(Produto.Id);
                await CarregarFornecedoresAsync(Produto.FornecedorId);
                return Page();
            }

            var produtoExistente = await _context.Produtos.FirstOrDefaultAsync(p => p.Id == Produto.Id);

            if (produtoExistente == null)
            {
                return NotFound();
            }

            produtoExistente.Nome = Produto.Nome;
            produtoExistente.Descricao = Produto.Descricao;
            produtoExistente.Preco = Produto.Preco;
            produtoExistente.Stock = Produto.Stock;
            produtoExistente.Categoria = Produto.Categoria;
            produtoExistente.MarcaVeiculo = Produto.MarcaVeiculo;
            produtoExistente.ModeloVeiculo = Produto.ModeloVeiculo;
            produtoExistente.FornecedorId = Produto.FornecedorId;

            if (UploadImagens != null && UploadImagens.Count > 0)
            {
                string pastaRelativa = Path.Combine("images", "produtos");
                string pastaFisica = Path.Combine(_environment.WebRootPath, pastaRelativa);

                if (!Directory.Exists(pastaFisica))
                {
                    Directory.CreateDirectory(pastaFisica);
                }

                int proximaOrdem = await _context.ImagensEntidade
                    .Where(i => i.TipoEntidade == "Produto" && i.EntidadeId == Produto.Id)
                    .Select(i => (int?)i.Ordem)
                    .MaxAsync() ?? -1;

                proximaOrdem++;

                bool jaExistePrincipal = await _context.ImagensEntidade
                    .AnyAsync(i => i.TipoEntidade == "Produto" && i.EntidadeId == Produto.Id && i.Principal);

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

                    _context.ImagensEntidade.Add(new ImagemEntidade
                    {
                        Url = $"/images/produtos/{nomeFicheiro}",
                        Alt = Produto.Nome,
                        Ordem = proximaOrdem,
                        Principal = !jaExistePrincipal && proximaOrdem == 0,
                        EntidadeId = Produto.Id,
                        TipoEntidade = "Produto"
                    });

                    proximaOrdem++;
                }
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Produto atualizado com sucesso.";
            return RedirectToPage("./Index");
        }

        /// <summary>
        /// Remove uma imagem individual da galeria do produto.
        /// </summary>
        /// <param name="id">Identificador do produto.</param>
        /// <param name="imagemId">Identificador da imagem.</param>
        /// <returns>Redireciona novamente para a página de edição.</returns>
        public async Task<IActionResult> OnPostDeleteImagemAsync(int id, int imagemId)
        {
            var imagem = await _context.ImagensEntidade
                .FirstOrDefaultAsync(i => i.Id == imagemId && i.TipoEntidade == "Produto" && i.EntidadeId == id);

            if (imagem == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(imagem.Url))
            {
                string caminhoRelativo = imagem.Url.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
                string caminhoFisico = Path.Combine(_environment.WebRootPath, caminhoRelativo);

                if (System.IO.File.Exists(caminhoFisico))
                {
                    System.IO.File.Delete(caminhoFisico);
                }
            }

            bool eraPrincipal = imagem.Principal;

            _context.ImagensEntidade.Remove(imagem);
            await _context.SaveChangesAsync();

            if (eraPrincipal)
            {
                var novaPrincipal = await _context.ImagensEntidade
                    .Where(i => i.TipoEntidade == "Produto" && i.EntidadeId == id)
                    .OrderBy(i => i.Ordem)
                    .FirstOrDefaultAsync();

                if (novaPrincipal != null)
                {
                    novaPrincipal.Principal = true;
                    await _context.SaveChangesAsync();
                }
            }

            TempData["SuccessMessage"] = "Imagem removida com sucesso.";
            return RedirectToPage(new { id });
        }

        /// <summary>
        /// Define uma imagem como principal.
        /// </summary>
        /// <param name="id">Identificador do produto.</param>
        /// <param name="imagemId">Identificador da imagem.</param>
        /// <returns>Redireciona novamente para a página de edição.</returns>
        public async Task<IActionResult> OnPostDefinirPrincipalAsync(int id, int imagemId)
        {
            var imagens = await _context.ImagensEntidade
                .Where(i => i.TipoEntidade == "Produto" && i.EntidadeId == id)
                .ToListAsync();

            if (!imagens.Any())
            {
                return NotFound();
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
        /// Obtém as imagens do produto.
        /// </summary>
        /// <param name="produtoId">Identificador do produto.</param>
        /// <returns>Lista de imagens do produto.</returns>
        private async Task<List<ImagemEntidade>> ObterImagensProdutoAsync(int produtoId)
        {
            return await _context.ImagensEntidade
                .AsNoTracking()
                .Where(i => i.TipoEntidade == "Produto" && i.EntidadeId == produtoId)
                .OrderByDescending(i => i.Principal)
                .ThenBy(i => i.Ordem)
                .ToListAsync();
        }

        /// <summary>
        /// Carrega a lista de fornecedores e seleciona o atual, se existir.
        /// </summary>
        /// <param name="fornecedorSelecionadoId">Fornecedor atualmente selecionado.</param>
        private async Task CarregarFornecedoresAsync(int? fornecedorSelecionadoId = null)
        {
            var fornecedores = await _context.Fornecedores
                .AsNoTracking()
                .OrderBy(f => f.Nome)
                .ToListAsync();

            FornecedoresSelect = new SelectList(
                fornecedores,
                "Id",
                "Nome",
                fornecedorSelecionadoId);
        }
    }
}