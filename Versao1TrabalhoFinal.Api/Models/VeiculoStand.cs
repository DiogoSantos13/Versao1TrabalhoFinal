using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Versao1TrabalhoFinal.Models
{
    /// <summary>
    /// Representa um veículo disponível no stand.
    /// </summary>
    public class VeiculoStand
    {
        /// <summary>
        /// Identificador do registo do stand.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador do veículo associado.
        /// </summary>
        [Required]
        public int VeiculoId { get; set; }

        /// <summary>
        /// Veículo associado ao registo do stand.
        /// </summary>
        public Veiculo? Veiculo { get; set; }

        /// <summary>
        /// Preço do veículo.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal Preco { get; set; }

        /// <summary>
        /// Estado atual do veículo no stand.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Estado { get; set; } = string.Empty;
    }
}