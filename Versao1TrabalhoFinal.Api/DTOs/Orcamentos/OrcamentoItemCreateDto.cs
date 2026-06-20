using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Api.DTOs.Orcamentos
{
    /// <summary>
    /// DTO para criação de itens de orçamento.
    /// </summary>
    public class OrcamentoItemCreateDto
    {
        public int? ProdutoId { get; set; }

        public int? ServicoId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantidade { get; set; }

        public decimal ValorTotal { get; set; } = 0;

        [Range(0, double.MaxValue)]
        public decimal PrecoUnitario { get; set; }
    }
}