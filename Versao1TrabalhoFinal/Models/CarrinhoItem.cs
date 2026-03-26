using System.ComponentModel.DataAnnotations.Schema;

namespace Versao1TrabalhoFinal.Models
{
    /// <summary>
    /// Representa um item do carrinho associado a um veículo do stand.
    /// </summary>
    public class CarrinhoItem
    {
        /// <summary>
        /// Identificador do item do carrinho.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador do carrinho.
        /// </summary>
        public int CarrinhoId { get; set; }

        /// <summary>
        /// Identificador do veículo do stand.
        /// </summary>
        public int VeiculoStandId { get; set; }

        /// <summary>
        /// Preço do veículo no momento em que foi adicionado ao carrinho.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoNoMomento { get; set; }

        /// <summary>
        /// Data em que o item foi adicionado ao carrinho.
        /// </summary>
        public DateTime DataAdicao { get; set; } = DateTime.Now;

        /// <summary>
        /// Carrinho associado ao item.
        /// </summary>
        public Carrinho Carrinho { get; set; } = null!;

        /// <summary>
        /// Veículo do stand associado ao item.
        /// </summary>
        public VeiculoStand VeiculoStand { get; set; } = null!;
    }
}
