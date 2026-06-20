using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Versao1TrabalhoFinal.Models
{
    /// <summary>
    /// Representa um orçamento associado a um cliente e a um veículo.
    /// </summary>
    public class Orcamento
    {
        /// <summary>
        /// Identificador único do orçamento.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Identificador do cliente associado ao orçamento.
        /// </summary>
        [Required]
        public int ClienteId { get; set; }

        /// <summary>
        /// Cliente associado ao orçamento.
        /// </summary>
        [ForeignKey(nameof(ClienteId))]
        public Cliente? Cliente { get; set; }

        /// <summary>
        /// Identificador do veículo associado ao orçamento.
        /// </summary>
        [Required]
        public int VeiculoId { get; set; }

        /// <summary>
        /// Veículo associado ao orçamento.
        /// </summary>
        [ForeignKey(nameof(VeiculoId))]
        public Veiculo? Veiculo { get; set; }

        /// <summary>
        /// Observações adicionais do orçamento.
        /// </summary>
        [MaxLength(1000)]
        public string? Observacoes { get; set; }

        /// <summary>
        /// Valor total do orçamento.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        /// <summary>
        /// Data de criação do orçamento.
        /// </summary>
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Coleção de itens associados ao orçamento.
        /// </summary>
        public ICollection<OrcamentoItem> Itens { get; set; } = new List<OrcamentoItem>();
    }
}