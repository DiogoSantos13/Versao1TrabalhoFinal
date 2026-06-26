namespace Versao1TrabalhoFinal.Models.PainelEmpresa
{
    /// <summary>
    /// Representa o detalhe operacional de uma ordem de reparação.
    /// </summary>
    public class PainelOrdemDetalheViewModel
    {
        /// <summary>
        /// Identificador da ordem.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome do cliente.
        /// </summary>
        public string ClienteNome { get; set; } = string.Empty;

        /// <summary>
        /// Descrição do veículo.
        /// </summary>
        public string VeiculoDescricao { get; set; } = string.Empty;

        /// <summary>
        /// Problema reportado.
        /// </summary>
        public string ProblemaReportado { get; set; } = string.Empty;

        /// <summary>
        /// Estado atual.
        /// </summary>
        public string Estado { get; set; } = string.Empty;

        /// <summary>
        /// Data de entrada.
        /// </summary>
        public DateTime DataEntrada { get; set; }

        /// <summary>
        /// Data de saída, se existir.
        /// </summary>
        public DateTime? DataSaida { get; set; }

        /// <summary>
        /// Total da ordem.
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Mecânico responsável.
        /// </summary>
        public string MecanicoResponsavel { get; set; } = "Por atribuir";

        /// <summary>
        /// Lista textual de serviços.
        /// </summary>
        public List<string> Servicos { get; set; } = new();
    }
}