using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Api.DTOs.Orcamentos
{
    /// <summary>
    /// DTO para atualização de orçamentos.
    /// </summary>
    public class OrcamentoUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int ClienteId { get; set; }

        public decimal ValorTotal { get; set; } = 0;

        [Required]
        public int VeiculoId { get; set; }

        public string? Observacoes { get; set; }
    }
}