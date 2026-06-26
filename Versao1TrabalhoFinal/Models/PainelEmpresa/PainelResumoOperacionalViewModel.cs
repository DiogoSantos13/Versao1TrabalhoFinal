namespace Versao1TrabalhoFinal.Models.PainelEmpresa
{
    /// <summary>
    /// Agrupa pequenos resumos operacionais para a página principal do painel.
    /// </summary>
    public class PainelResumoOperacionalViewModel
    {
        /// <summary>
        /// Número de ordens pendentes.
        /// </summary>
        public int OrdensPendentes { get; set; }

        /// <summary>
        /// Número de ordens em reparação.
        /// </summary>
        public int OrdensEmReparacao { get; set; }

        /// <summary>
        /// Número de orçamentos pendentes.
        /// </summary>
        public int OrcamentosPendentes { get; set; }

        /// <summary>
        /// Número de vendas do dia.
        /// </summary>
        public int VendasHoje { get; set; }
    }
}