namespace Versao1TrabalhoFinal.Api.DTOs.Orcamentos
{
    /// <summary>
    /// DTO de leitura de item de orçamento.
    /// </summary>
    public class OrcamentoItemReadDto
    {
        public int Id { get; set; }

        public int OrcamentoId { get; set; }

        public int? ProdutoId { get; set; }

        public decimal ValorTotal { get; set; } = 0;
        public string? ProdutoNome { get; set; }

        public int? ServicoId { get; set; }

        public string? ServicoNome { get; set; }

        public int Quantidade { get; set; }

        public decimal PrecoUnitario { get; set; }
    }
}