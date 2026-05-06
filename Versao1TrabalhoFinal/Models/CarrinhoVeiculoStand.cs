using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Models
{
    /// <summary>
    /// Representa a associação entre um carrinho e um veículo disponível no stand.
    /// </summary>
    public class CarrinhoVeiculoStand
    {
        /// <summary>
        /// Identificador único do registo de associação.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador do carrinho ao qual o veículo foi associado.
        /// </summary>
        [Required]
        public int CarrinhoId { get; set; }

        /// <summary>
        /// Navegação para o carrinho associado.
        /// </summary>
        public Carrinho Carrinho { get; set; } = null!;

        /// <summary>
        /// Identificador do veículo do stand associado ao carrinho.
        /// </summary>
        [Required]
        public int VeiculoStandId { get; set; }

        /// <summary>
        /// Navegação para o veículo do stand associado.
        /// </summary>
        public VeiculoStand VeiculoStand { get; set; } = null!;

        /// <summary>
        /// Preço do veículo no momento em que foi adicionado ao carrinho.
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "O preço no momento tem de ser igual ou superior a zero.")]
        public decimal PrecoNoMomento { get; set; }

        /// <summary>
        /// Data e hora em que o veículo foi adicionado ao carrinho.
        /// </summary>
        public DateTime DataAdicao { get; set; } = DateTime.Now;
    }
}