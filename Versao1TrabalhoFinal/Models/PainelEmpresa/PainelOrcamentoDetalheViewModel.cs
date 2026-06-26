namespace Versao1TrabalhoFinal.Models.PainelEmpresa
{
    /// <summary>
    /// Representa o detalhe visual de um orçamento no painel.
    /// </summary>
    public class PainelOrcamentoDetalheViewModel
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
        /// Veículo associado.
        /// </summary>
        public string VeiculoDescricao { get; set; } = string.Empty;

        /// <summary>
        /// Descrição do pedido.
        /// </summary>
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Estado atual.
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

        /// <summary>
        /// Indica se foi gerado por IA.
        /// </summary>
        public bool? GeradoPorIA { get; set; }
    }
}