using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models.PainelEmpresa;

namespace Versao1TrabalhoFinal.Services
{
    /// <summary>
    /// Serviço responsável por centralizar a lógica do painel interno da empresa.
    /// </summary>
    public class PainelEmpresaService : IPainelEmpresaService
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Inicializa o serviço do painel.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public PainelEmpresaService(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Carrega todos os dados necessários ao painel principal.
        /// </summary>
        /// <returns>ViewModel completo.</returns>
        public async Task<PainelEmpresaViewModel> ObterPainelAsync()
        {
            var ordens = await _context.OrdensReparacao
                .Include(o => o.Cliente)
                .Include(o => o.Veiculo)
                .OrderByDescending(o => o.DataEntrada)
                .Take(20)
                .ToListAsync();

            var orcamentos = await _context.Orcamentos
                .Include(o => o.Cliente)
                .Include(o => o.Veiculo)
                .OrderByDescending(o => o.DataCriacao)
                .Take(20)
                .ToListAsync();

            /// <summary>
            /// IMPORTANTE:
            /// Se a tabela VendaItem ainda estiver com problema de mapeamento,
            /// usa temporariamente apenas Include(v => v.Cliente).
            /// </summary>
            var vendas = await _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.Itens)
                    .ThenInclude(i => i.Produto)
                .OrderByDescending(v => v.DataVenda)
                .Take(20)
                .ToListAsync();

            var totalVendas = await _context.Vendas.SumAsync(v => (decimal?)v.Total) ?? 0;
            var totalVendasFinalizadas = await _context.Vendas.CountAsync();
            var totalOrcamentosPendentes = await _context.Orcamentos.CountAsync(o => o.Estado == "Pendente");
            var totalOrdensAbertas = await _context.OrdensReparacao.CountAsync(o => o.Estado != "Concluído" && o.Estado != "Entregue");

            var painel = new PainelEmpresaViewModel
            {
                TotalVendas = totalVendas,
                TotalVendasFinalizadas = totalVendasFinalizadas,
                TotalOrcamentosPendentes = totalOrcamentosPendentes,
                TotalOrdensAbertas = totalOrdensAbertas,

                OrdensReparacao = ordens.Select(o => new PainelOrdemReparacaoViewModel
                {
                    Id = o.Id,
                    ClienteNome = o.Cliente != null ? o.Cliente.Nome : "Sem cliente",
                    VeiculoDescricao = PainelFormatadorHelper.FormatarVeiculo(o.Veiculo),
                    ServicosPedidos = PainelFormatadorHelper.TextoOuPadrao(o.DescricaoProblema, "Sem descrição"),
                    MecanicoResponsavel = "Por atribuir",
                    Estado = PainelFormatadorHelper.TextoOuPadrao(o.Estado, "Pendente"),
                    DataEntrada = o.DataEntrada,
                    Total = o.Total
                }).ToList(),

                Orcamentos = orcamentos.Select(o => new PainelOrcamentoViewModel
                {
                    Id = o.Id,
                    ClienteNome = o.Cliente != null ? o.Cliente.Nome : "Sem cliente",
                    VeiculoDescricao = PainelFormatadorHelper.FormatarVeiculo(o.Veiculo),
                    Descricao = o.Descricao,
                    Estado = PainelFormatadorHelper.TextoOuPadrao(o.Estado, "Pendente"),
                    ValorEstimado = o.ValorEstimado,
                    DataCriacao = o.DataCriacao
                }).ToList(),

                Vendas = vendas.Select(v => new PainelVendaViewModel
                {
                    Id = v.Id,
                    ClienteNome = v.Cliente != null ? v.Cliente.Nome : "Sem cliente",
                    DataVenda = v.DataVenda,
                    Total = v.Total,
                    Tipo = PainelFormatadorHelper.TextoOuPadrao(v.Tipo, "Venda"),
                    ItensResumo = v.Itens != null && v.Itens.Any()
                        ? string.Join(", ", v.Itens.Select(i =>
                            $"{(i.Produto != null ? i.Produto.Nome : "Produto")} x{i.Quantidade}"))
                        : "Sem itens"
                }).ToList(),

                Kpis = new List<PainelKpiViewModel>
                {
                    new PainelKpiViewModel
                    {
                        Titulo = "Total de vendas",
                        Valor = totalVendas.ToString("C"),
                        Subtitulo = "Valor acumulado",
                        Icone = "fa-solid fa-sack-dollar",
                        ClasseCss = "kpi-primary"
                    },
                    new PainelKpiViewModel
                    {
                        Titulo = "Vendas finalizadas",
                        Valor = totalVendasFinalizadas.ToString(),
                        Subtitulo = "Histórico total",
                        Icone = "fa-solid fa-cart-shopping",
                        ClasseCss = "kpi-success"
                    },
                    new PainelKpiViewModel
                    {
                        Titulo = "Orçamentos pendentes",
                        Valor = totalOrcamentosPendentes.ToString(),
                        Subtitulo = "Aguardam análise",
                        Icone = "fa-solid fa-file-invoice",
                        ClasseCss = "kpi-warning"
                    },
                    new PainelKpiViewModel
                    {
                        Titulo = "Ordens em aberto",
                        Valor = totalOrdensAbertas.ToString(),
                        Subtitulo = "Em curso ou por concluir",
                        Icone = "fa-solid fa-screwdriver-wrench",
                        ClasseCss = "kpi-info"
                    }
                },

                ResumoOperacional = new PainelResumoOperacionalViewModel
                {
                    OrdensPendentes = await _context.OrdensReparacao.CountAsync(o => o.Estado == "Pendente"),
                    OrdensEmReparacao = await _context.OrdensReparacao.CountAsync(o => o.Estado == "Em Reparação"),
                    OrcamentosPendentes = totalOrcamentosPendentes,
                    VendasHoje = await _context.Vendas.CountAsync(v => v.DataVenda.Date == DateTime.Today)
                },

                AtividadesRecentes = ordens.Take(5).Select(o => new PainelAtividadeRecenteViewModel
                {
                    Tipo = "Ordem",
                    Titulo = $"Ordem #{o.Id}",
                    Descricao = $"{(o.Cliente != null ? o.Cliente.Nome : "Sem cliente")} - {PainelFormatadorHelper.TextoOuPadrao(o.Estado, "Pendente")}",
                    Data = o.DataEntrada,
                    ClasseCss = "status-neutral"
                }).ToList()
            };

            return painel;
        }

        /// <summary>
        /// Atualiza o estado e o valor estimado de um orçamento.
        /// </summary>
        /// <param name="input">Dados da atualização.</param>
        /// <returns>Resultado da operação.</returns>
        public async Task<PainelOperacaoResultado> AtualizarOrcamentoAsync(AtualizarOrcamentoInputModel input)
        {
            var orcamento = await _context.Orcamentos.FirstOrDefaultAsync(o => o.Id == input.OrcamentoId);

            if (orcamento == null)
            {
                return PainelOperacaoResultado.Falha("O orçamento não foi encontrado.", "ORCAMENTO_NAO_ENCONTRADO");
            }

            orcamento.Estado = input.NovoEstado;
            orcamento.ValorEstimado = Convert.ToDecimal(input.ValorEstimado);

            await _context.SaveChangesAsync();

            return PainelOperacaoResultado.Ok("Estado do orçamento atualizado com sucesso.");
        }

        /// <summary>
        /// Atualiza o estado de uma ordem de reparação.
        /// </summary>
        /// <param name="input">Dados da atualização.</param>
        /// <returns>Resultado da operação.</returns>
        public async Task<PainelOperacaoResultado> AtualizarOrdemAsync(AtualizarOrdemInputModel input)
        {
            var ordem = await _context.OrdensReparacao.FirstOrDefaultAsync(o => o.Id == input.OrdemReparacaoId);

            if (ordem == null)
            {
                return PainelOperacaoResultado.Falha("A ordem de reparação não foi encontrada.", "ORDEM_NAO_ENCONTRADA");
            }

            ordem.Estado = input.NovoEstado;

            await _context.SaveChangesAsync();

            return PainelOperacaoResultado.Ok("Estado da ordem de reparação atualizado com sucesso.");
        }
    }
}