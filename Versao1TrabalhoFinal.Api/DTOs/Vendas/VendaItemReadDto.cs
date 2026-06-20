namespace Versao1TrabalhoFinal.Api.DTOs.Vendas
{
    /// <summary>
    /// DTO de leitura de itens de venda.
    /// </summary>
    public class VendaItemReadDto
    {
        public int Id { get; set; }

        public int VendaId { get; set; }

        public int? ProdutoId { get; set; }

        public string? ProdutoNome { get; set; }

        public int? ServicoId { get; set; }

        public string? ServicoNome { get; set; }

        public int Quantidade { get; set; }

        public decimal PrecoUnitario { get; set; }
    }
}