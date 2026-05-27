using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Pages.Servicos
{
    /// <summary>
    /// Página responsável pela listagem de serviços.
    /// </summary>
    public class IndexModel : PageModel
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Inicializa uma nova instância da página de listagem de serviços.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public IndexModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista de serviços.
        /// </summary>
        public List<Servico> Servicos { get; set; } = new();

        /// <summary>
        /// Carrega a lista de serviços.
        /// </summary>
        public async Task OnGetAsync()
        {
            Servicos = await _context.Servicos
                .AsNoTracking()
                .OrderBy(s => s.Nome)
                .ToListAsync();

            var ids = Servicos.Select(s => s.Id).ToList();

            if (ids.Any())
            {
                var imagens = await _context.ImagensEntidade
                    .AsNoTracking()
                    .Where(i => i.TipoEntidade == "Servico" && ids.Contains(i.EntidadeId))
                    .OrderByDescending(i => i.Principal)
                    .ThenBy(i => i.Ordem)
                    .ToListAsync();

                foreach (var servico in Servicos)
                {
                    servico.Imagens = imagens
                        .Where(i => i.EntidadeId == servico.Id)
                        .OrderByDescending(i => i.Principal)
                        .ThenBy(i => i.Ordem)
                        .ToList();
                }
            }
        }
    }
}