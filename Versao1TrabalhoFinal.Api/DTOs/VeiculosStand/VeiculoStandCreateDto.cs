using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Api.DTOs.VeiculosStand
{
    /// <summary>
    /// DTO para criação de veículos do stand.
    /// </summary>
    public class VeiculoStandCreateDto
    {
        /// <summary>
        /// Identificador do veículo associado.
        /// </summary>
        [Required]
        public int VeiculoId { get; set; }

        /// <summary>
        /// Preço de venda do veículo.
        /// </summary>
        [Range(0, double.MaxValue)]
        public decimal Preco { get; set; }

        /// <summary>
        /// Estado atual do veículo no stand.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Estado { get; set; } = string.Empty;
    }
}