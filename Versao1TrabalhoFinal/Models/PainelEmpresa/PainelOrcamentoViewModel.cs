namespace Versao1TrabalhoFinal.Models.PainelEmpresa
{
    /// <summary>
    /// Representa um orçamento mostrado no painel da empresa.
    /// </summary>
    public class PainelOrcamentoViewModel
    {
        /// <summary>
        /// Identificador do orçamento.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome do cliente.
        /// </summary>
        public string ClienteNome { get; set; } = string.Empty;

        /// <summary>
        /// Veículo associado ao orçamento.
        /// </summary>
        public string VeiculoDescricao { get; set; } = string.Empty;

        /// <summary>
        /// Descrição do orçamento.
        /// </summary>
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Estado atual do orçamento.
        /// </summary>
        public string Estado { get; set; } = string.Empty;

        /// <summary>
        /// Valor estimado.
        /// </summary>
        public decimal? ValorEstimado { get; set; }

        /// <summary>
        /// Data de criação.
        /// </summary>
        public DateTime? DataCriacao { get; set; }
    }
}