using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Models
{
    /// <summary>
    /// Representa um pedido de orçamento submetido por um cliente.
    /// </summary>
    public class Orcamento
    {
        /// <summary>
        /// Identificador único do pedido de orçamento.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador do cliente associado ao pedido.
        /// </summary>
        [Display(Name = "Cliente")]
        public int? ClienteId { get; set; }

        /// <summary>
        /// Cliente associado ao pedido.
        /// </summary>
        public Cliente? Cliente { get; set; }

        /// <summary>
        /// Identificador do veículo associado ao pedido.
        /// </summary>
        [Display(Name = "Veículo")]
        public int? VeiculoId { get; set; }

        /// <summary>
        /// Veículo associado ao pedido.
        /// </summary>
        public Veiculo? Veiculo { get; set; }

        /// <summary>
        /// Descrição do problema indicado pelo cliente.
        /// </summary>
        [Required(ErrorMessage = "A descrição do problema é obrigatória.")]
        [StringLength(500, ErrorMessage = "A descrição não pode ultrapassar os 500 caracteres.")]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Valor estimado atribuído posteriormente por um administrador ou colaborador.
        /// </summary>
        [Display(Name = "Valor estimado")]
        public decimal? ValorEstimado { get; set; }

        /// <summary>
        /// Indica se o orçamento foi gerado com recurso a inteligência artificial.
        /// </summary>
        [Display(Name = "Gerado por IA")]
        public bool? GeradoPorIA { get; set; }

        /// <summary>
        /// Data de criação do pedido.
        /// </summary>
        [Display(Name = "Data de criação")]
        public DateTime? DataCriacao { get; set; } = DateTime.Now;

        /// <summary>
        /// Estado atual do pedido de orçamento.
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Estado")]
        public string? Estado { get; set; } = "Pendente";
    }
}
