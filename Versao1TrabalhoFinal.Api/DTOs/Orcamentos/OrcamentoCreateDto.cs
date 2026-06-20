using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Api.DTOs.Orcamentos
{
    /// <summary>
    /// DTO para criação de orçamentos.
    /// </summary>
    public class OrcamentoCreateDto
    {
        [Required]
        public int ClienteId { get; set; }

        [Required]
        public int VeiculoId { get; set; }

        public string? Observacoes { get; set; }

        public decimal ValorTotal { get; set; } = 0;
        public List<OrcamentoItemCreateDto> Itens { get; set; } = new();
    }
}