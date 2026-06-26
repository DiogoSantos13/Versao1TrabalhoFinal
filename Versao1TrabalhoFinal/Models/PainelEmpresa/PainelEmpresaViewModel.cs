namespace Versao1TrabalhoFinal.Models.PainelEmpresa
{
    /// <summary>
    /// ViewModel principal do painel interno da empresa.
    /// Agrega métricas, ordens de reparação, orçamentos, vendas e atividade recente.
    /// </summary>
    public class PainelEmpresaViewModel
    {
        /// <summary>
        /// Soma total das vendas registadas.
        /// </summary>
        public decimal TotalVendas { get; set; }

        /// <summary>
        /// Número de vendas finalizadas.
        /// </summary>
        public int TotalVendasFinalizadas { get; set; }

        /// <summary>
        /// Número de orçamentos pendentes.
        /// </summary>
        public int TotalOrcamentosPendentes { get; set; }

        /// <summary>
        /// Número de ordens de reparação em aberto.
        /// </summary>
        public int TotalOrdensAbertas { get; set; }

        /// <summary>
        /// Cartões KPI prontos a mostrar na interface.
        /// </summary>
        public List<PainelKpiViewModel> Kpis { get; set; } = new();

        /// <summary>
        /// Lista de ordens de reparação para trabalho diário.
        /// </summary>
        public List<PainelOrdemReparacaoViewModel> OrdensReparacao { get; set; } = new();

        /// <summary>
        /// Lista de orçamentos recentes.
        /// </summary>
        public List<PainelOrcamentoViewModel> Orcamentos { get; set; } = new();

        /// <summary>
        /// Histórico de vendas finalizadas.
        /// </summary>
        public List<PainelVendaViewModel> Vendas { get; set; } = new();

        /// <summary>
        /// Lista de atividade recente da empresa.
        /// </summary>
        public List<PainelAtividadeRecenteViewModel> AtividadesRecentes { get; set; } = new();

        /// <summary>
        /// Resumo dos principais clientes.
        /// </summary>
        public List<PainelResumoClienteViewModel> TopClientes { get; set; } = new();
       
        /// <summary>
        /// Resumo operacional curto.
        /// </summary>
        public PainelResumoOperacionalViewModel ResumoOperacional { get; set; } = new();

        /// <summary>
        /// Lista de carrinhos dos clientes para consulta no painel.
        /// </summary>
        public List<PainelCarrinhoViewModel> Carrinhos { get; set; } = new();
    }
}