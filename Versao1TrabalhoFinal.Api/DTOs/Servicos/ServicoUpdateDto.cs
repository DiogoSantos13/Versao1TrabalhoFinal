using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Api.DTOs.Servicos
{
    /// <summary>
    /// DTO para atualização de serviços.
    /// </summary>
    public class ServicoUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;

        public string? Descricao { get; set; }

        [Range(0, double.MaxValue)]
        public decimal PrecoBase { get; set; }

        public int? DuracaoEstimadaMinutos { get; set; }
    }
}