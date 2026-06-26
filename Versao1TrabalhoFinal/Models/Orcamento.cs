using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Models
{
    /// <summary>
    /// Representa um orçamento pedido por um cliente.
    /// </summary>
    public class Orcamento
    {
        public int Id { get; set; }

        [Display(Name = "Cliente")]
        public int? ClienteId { get; set; }

        public Cliente? Cliente { get; set; }

        [Display(Name = "Veículo")]
        public int? VeiculoId { get; set; }

        public Veiculo? Veiculo { get; set; }

        [Required(ErrorMessage = "A descrição do problema é obrigatória.")]
        [StringLength(500, ErrorMessage = "A descrição não pode ultrapassar os 500 caracteres.")]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; } = string.Empty;

        [Display(Name = "Valor estimado")]
        public decimal? ValorEstimado { get; set; }

        [Display(Name = "Gerado por IA")]
        public bool? GeradoPorIA { get; set; }

        [Display(Name = "Data de criação")]
        public DateTime? DataCriacao { get; set; } = DateTime.Now;

        [StringLength(50)]
        [Display(Name = "Estado")]
        public string? Estado { get; set; } = "Pendente";
    }
}