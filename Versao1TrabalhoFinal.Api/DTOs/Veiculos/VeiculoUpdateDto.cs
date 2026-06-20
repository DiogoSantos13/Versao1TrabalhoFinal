using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Api.DTOs.Veiculos
{
    /// <summary>
    /// DTO para atualização de veículos.
    /// </summary>
    public class VeiculoUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Marca { get; set; } = string.Empty;

        [Required]
        public string Modelo { get; set; } = string.Empty;

        [Required]
        public string Matricula { get; set; } = string.Empty;

        public int Ano { get; set; }

        public string? Cor { get; set; }

        public string? Combustivel { get; set; }

        public int Quilometros { get; set; }

        public int? ClienteId { get; set; }
    }
}