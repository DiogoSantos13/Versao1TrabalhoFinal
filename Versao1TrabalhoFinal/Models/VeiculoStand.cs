using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Versao1TrabalhoFinal.Models
{
    [Table("VeiculoStand")]
    public class VeiculoStand : EntidadeComGaleria
    {
        [Key]
        public int Id { get; set; }
        
        public int? VeiculoId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Preco { get; set; }

        public string? Estado { get; set; }

        public int Quilometros { get; set; }

        public string? Descricao { get; set; }

        public DateTime DataEntrada { get; set; }

        public int? Cilindrada { get; set; }

     //   public string? ImagemUrl { get; set; }

        [ForeignKey(nameof(VeiculoId))]
        public Veiculo? Veiculo { get; set; }

        /// <summary>
        /// Registos de associação entre este veículo do stand e carrinhos.
        /// </summary>
        public ICollection<CarrinhoVeiculoStand> CarrinhoVeiculosStand { get; set; } = new List<CarrinhoVeiculoStand>();
    }
}
