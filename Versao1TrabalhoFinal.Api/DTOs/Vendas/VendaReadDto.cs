namespace Versao1TrabalhoFinal.Api.DTOs.Vendas
{
    /// <summary>
    /// DTO de leitura de vendas.
    /// </summary>
    public class VendaReadDto
    {
        public int Id { get; set; }

        public int ClienteId { get; set; }

        public string? ClienteNome { get; set; }

        public decimal Total { get; set; }

        public string? Observacoes { get; set; }

        public DateTime DataVenda { get; set; }

        public List<VendaItemReadDto> Itens { get; set; } = new();
    }
}