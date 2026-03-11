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
    /// Página de listagem de produtos com filtros por marca, modelo e texto.
    /// </summary>
    [Authorize(Roles = "Admin,Colaborador")]
    public class IndexModel : PageModel
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Construtor da página.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public IndexModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Filtro de marca do veículo.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? MarcaFiltro { get; set; }

        /// <summary>
        /// Filtro de modelo do veículo.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? ModeloFiltro { get; set; }

        /// <summary>
        /// Texto de pesquisa livre.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? Pesquisa { get; set; }

        /// <summary>
        /// Lista final de produtos.
        /// </summary>
        public List<Produto> Produtos { get; set; } = new();

        /// <summary>
        /// Lista de marcas disponíveis para filtro.
        /// </summary>
        public List<SelectListItem> Marcas { get; set; } = new();

        /// <summary>
        /// Lista de modelos disponíveis para filtro.
        /// </summary>
        public List<SelectListItem> Modelos { get; set; } = new();

        /// <summary>
        /// Carrega produtos e filtros.
        /// </summary>
        public async Task OnGetAsync()
        {
            Marcas = await _context.Veiculos
                .Where(v => v.Marca != null)
                .Select(v => v.Marca!)
                .Distinct()
                .OrderBy(x => x)
                .Select(x => new SelectListItem { Value = x, Text = x })
                .ToListAsync();

            var modelosQuery = _context.Veiculos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(MarcaFiltro))
            {
                modelosQuery = modelosQuery.Where(v => v.Marca == MarcaFiltro);
            }

            Modelos = await modelosQuery
                .Where(v => v.Modelo != null)
                .Select(v => v.Modelo!)
                .Distinct()
                .OrderBy(x => x)
                .Select(x => new SelectListItem { Value = x, Text = x })
                .ToListAsync();

            var query = _context.Produtos
                .Include(p => p.Fornecedor)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(MarcaFiltro))
                query = query.Where(p => p.MarcaVeiculo == MarcaFiltro);

            if (!string.IsNullOrWhiteSpace(ModeloFiltro))
                query = query.Where(p => p.ModeloVeiculo == ModeloFiltro);

            if (!string.IsNullOrWhiteSpace(Pesquisa))
            {
                query = query.Where(p =>
                    (p.Nome != null && p.Nome.Contains(Pesquisa)) ||
                    (p.Descricao != null && p.Descricao.Contains(Pesquisa)) ||
                    (p.Categoria != null && p.Categoria.Contains(Pesquisa)));
            }

            Produtos = await query
                .OrderBy(p => p.Nome)
                .ToListAsync();
        }
    }
}
