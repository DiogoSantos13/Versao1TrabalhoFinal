namespace Versao1TrabalhoFinal.Models.PainelEmpresa
{
    /// <summary>
    /// Produto associado a uma ordem de reparação para mostrar no painel.
    /// </summary>
    public class PainelOrdemProdutoViewModel
    {
        /// <summary>
        /// ID do registo da associação.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID do produto.
        /// </summary>
        public int ProdutoId { get; set; }

        /// <summary>
        /// Nome do produto.
        /// </summary>
        public string ProdutoNome { get; set; } = string.Empty;

        /// <summary>
        /// Quantidade usada na ordem.
        /// </summary>
        public int Quantidade { get; set; }

        /// <summary>
        /// Preço unitário guardado na ordem.
        /// </summary>
        public decimal Preco { get; set; }

        /// <summary>
        /// Subtotal calculado.
        /// </summary>
        public decimal Subtotal => Quantidade * Preco;
    }
}