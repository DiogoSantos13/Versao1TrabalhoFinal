namespace Versao1TrabalhoFinal.Api.DTOs.Produtos
{
    /// <summary>
    /// DTO de leitura de produtos.
    /// </summary>
    public class ProdutoReadDto
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string? Descricao { get; set; }

        public decimal Preco { get; set; }

        public int Stock { get; set; }

        public int? FornecedorId { get; set; }

        public string? FornecedorNome { get; set; }
    }
}