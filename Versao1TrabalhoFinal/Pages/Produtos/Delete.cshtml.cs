using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Produtos
{
    /// <summary>
    /// Página responsável pela eliminação de produtos.
    /// </summary>
    [Authorize(Roles = "Admin,Colaborador")]
    public class DeleteModel : PageModel
    {
        private readonly StandDbContext _context;
        private readonly IWebHostEnvironment _environment;

        /// <summary>
        /// Inicializa uma nova instância da página de eliminação de produtos.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        /// <param name="environment">Ambiente da aplicação para acesso ao wwwroot.</param>
        public DeleteModel(StandDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        /// <summary>
        /// Produto a eliminar.
        /// </summary>
        [BindProperty]
        public Produto Produto { get; set; } = new();

        /// <summary>
        /// URL da imagem de pré-visualização do produto.
        /// </summary>
        public string ImagemPreview { get; set; } = "~/images/produtos/sem-imagem.png";

        /// <summary>
        /// Carrega o produto antes da confirmação da eliminação.
        /// </summary>
        /// <param name="id">Identificador do produto.</param>
        /// <returns>Página ou NotFound.</returns>
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var produto = await _context.Produtos
                .AsNoTracking()
                .Include(p => p.Fornecedor)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (produto == null)
            {
                return NotFound();
            }

            var imagemPrincipal = await _context.ImagensEntidade
                .AsNoTracking()
                .Where(i => i.TipoEntidade == "Produto" && i.EntidadeId == id)
                .OrderByDescending(i => i.Principal)
                .ThenBy(i => i.Ordem)
                .Select(i => i.Url)
                .FirstOrDefaultAsync();

            Produto = produto;
            ImagemPreview = !string.IsNullOrWhiteSpace(imagemPrincipal)
                ? imagemPrincipal
                : "~/images/produtos/sem-imagem.png";

            return Page();
        }

        /// <summary>
        /// Elimina o produto selecionado e as imagens associadas.
        /// </summary>
        /// <param name="id">Identificador do produto.</param>
        /// <returns>Redireciona para a listagem.</returns>
        public async Task<IActionResult> OnPostAsync(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);

            if (produto == null)
            {
                return NotFound();
            }

            var imagens = await _context.ImagensEntidade
                .Where(i => i.TipoEntidade == "Produto" && i.EntidadeId == id)
                .ToListAsync();

            foreach (var imagem in imagens)
            {
                if (!string.IsNullOrWhiteSpace(imagem.Url))
                {
                    var caminhoRelativo = imagem.Url.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
                    var caminhoFisico = Path.Combine(_environment.WebRootPath, caminhoRelativo);

                    if (System.IO.File.Exists(caminhoFisico))
                    {
                        System.IO.File.Delete(caminhoFisico);
                    }
                }
            }

            if (imagens.Any())
            {
                _context.ImagensEntidade.RemoveRange(imagens);
            }

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Produto eliminado com sucesso.";
            return RedirectToPage("./Index");
        }
    }
}