namespace Versao1TrabalhoFinal.Models.PainelEmpresa
{
    /// <summary>
    /// Representa um produto presente no carrinho de um cliente,
    /// para visualização no painel interno.
    /// </summary>
    public class PainelCarrinhoProdutoViewModel
    {
        /// <summary>
        /// Nome do produto.
        /// </summary>
        public string NomeProduto { get; set; } = string.Empty;

        /// <summary>
        /// Quantidade escolhida pelo cliente.
        /// </summary>
        public int Quantidade { get; set; }

        /// <summary>
        /// Preço guardado no momento em que foi adicionado ao carrinho.
        /// </summary>
        public decimal PrecoNoMomento { get; set; }

        /// <summary>
        /// Data em que o item foi adicionado ao carrinho.
        /// </summary>
        public DateTime DataAdicao { get; set; }
    }
}