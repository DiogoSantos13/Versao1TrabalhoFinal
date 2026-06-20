namespace Versao1TrabalhoFinal.Api.DTOs.Carrinho
{
    /// <summary>
    /// DTO utilizado para devolver dados de um item do carrinho.
    /// </summary>
    public class CarrinhoItemReadDto
    {
        /// <summary>
        /// Identificador do item.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador do produto, caso exista.
        /// </summary>
        public int? ProdutoId { get; set; }

        /// <summary>
        /// Nome do produto, caso exista.
        /// </summary>
        public string? ProdutoNome { get; set; }

        /// <summary>
        /// Identificador do serviço, caso exista.
        /// </summary>
        public int? ServicoId { get; set; }

        /// <summary>
        /// Nome do serviço, caso exista.
        /// </summary>
        public string? ServicoNome { get; set; }

        /// <summary>
        /// Quantidade do item.
        /// </summary>
        public int Quantidade { get; set; }

        /// <summary>
        /// Preço unitário do item.
        /// </summary>
        public decimal PrecoUnitario { get; set; }
    }
}