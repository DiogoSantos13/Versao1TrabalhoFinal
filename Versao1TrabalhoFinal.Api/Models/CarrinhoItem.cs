using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Versao1TrabalhoFinal.Models
{
    /// <summary>
    /// Representa um item do carrinho associado a um veículo.
    /// </summary>
    public class CarrinhoItem
    {
        /// <summary>
        /// Identificador do item do carrinho.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador do veículo associado.
        /// </summary>
        [Required]
        public int VeiculoId { get; set; }

        /// <summary>
        /// Veículo associado ao item do carrinho.
        /// </summary>
        public Veiculo? Veiculo { get; set; }

        /// <summary>
        /// Identificador do produto, quando aplicável.
        /// </summary>
        public int? ProdutoId { get; set; }

        /// <summary>
        /// Produto associado ao item, quando aplicável.
        /// </summary>
        public Produto? Produto { get; set; }

        /// <summary>
        /// Identificador do serviço, quando aplicável.
        /// </summary>
        public int? ServicoId { get; set; }

        /// <summary>
        /// Serviço associado ao item, quando aplicável.
        /// </summary>
        public Servico? Servico { get; set; }

        /// <summary>
        /// Quantidade do item.
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser superior a zero.")]
        public int Quantidade { get; set; } = 1;

        /// <summary>
        /// Preço unitário do item.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoUnitario { get; set; }

        /// <summary>
        /// Data de criação do item no carrinho.
        /// </summary>
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public IdentityUser? User { get; set; }
    }
}