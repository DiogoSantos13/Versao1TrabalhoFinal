namespace Versao1TrabalhoFinal.Models.PainelEmpresa
{
    /// <summary>
    /// Representa uma linha visual de trabalho da oficina no painel.
    /// </summary>
    public class PainelOrdemReparacaoViewModel
    {
        /// <summary>
        /// Identificador da ordem de reparação.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome do cliente.
        /// </summary>
        public string ClienteNome { get; set; } = string.Empty;

        /// <summary>
        /// Identificação amigável do veículo.
        /// Exemplo: BMW 320d - 11-AA-22.
        /// </summary>
        public string VeiculoDescricao { get; set; } = string.Empty;

        /// <summary>
        /// Descrição dos serviços pedidos.
        /// </summary>
        public string ServicosPedidos { get; set; } = string.Empty;

        /// <summary>
        /// Nome do mecânico ou colaborador responsável.
        /// </summary>
        public string? MecanicoResponsavel { get; set; }

        /// <summary>
        /// Estado atual do trabalho.
        /// </summary>
        public string Estado { get; set; } = string.Empty;

        /// <summary>
        /// Descrição do problema/trabalho.
        /// </summary>
        public string? DescricaoProblema { get; set; }

        /// <summary>
        /// Data de entrada da viatura.
        /// </summary>
        public DateTime DataEntrada { get; set; }

        /// <summary>
        /// Valor estimado ou total da ordem.
        /// </summary>
        public decimal Total { get; set; }
        public int? ColaboradorId { get; set; }
        public decimal MaoDeObra { get; set; }
        public decimal Pecas { get; set; }
        /// <summary>
        /// Produtos associados à ordem de reparação.
        /// </summary>
        public List<PainelOrdemProdutoViewModel> Produtos { get; set; } = new();
        public List<PainelCarrinhoViewModel> Carrinhos { get; set; } = new();
    }
}