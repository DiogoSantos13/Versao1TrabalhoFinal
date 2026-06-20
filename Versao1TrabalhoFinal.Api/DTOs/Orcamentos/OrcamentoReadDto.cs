namespace Versao1TrabalhoFinal.Api.DTOs.Orcamentos
{
    /// <summary>
    /// DTO de leitura de orçamentos.
    /// </summary>
    public class OrcamentoReadDto
    {
        public int Id { get; set; }

        public int ClienteId { get; set; }

        public string? ClienteNome { get; set; }

        public int VeiculoId { get; set; }

        public decimal ValorTotal { get; set; } = 0;
        public string? VeiculoDescricao { get; set; }

        public string? Observacoes { get; set; }

        public decimal Total { get; set; }

        public DateTime DataCriacao { get; set; }

        public List<OrcamentoItemReadDto> Itens { get; set; } = new();
    }
}