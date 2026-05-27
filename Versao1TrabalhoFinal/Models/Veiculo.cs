using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Versao1TrabalhoFinal.Models
{
    [Table("Veiculos")]
    public class Veiculo : EntidadeComGaleria
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Marca { get; set; } = string.Empty;

        [Required]
        public string Modelo { get; set; } = string.Empty;

        public int Ano { get; set; }

        public string? Tipo { get; set; }

        public int Cilindrada { get; set; }

        public string? Combustivel { get; set; }

        public string? Matricula { get; set; }

        public string? ImagemUrl { get; set; }

        public int? ClienteId { get; set; }

        [ForeignKey(nameof(ClienteId))]
        public Cliente? Cliente { get; set; } //algo que não é necessário para criar um veículo, mas que pode ser útil para exibir informações do cliente associado ao veículo<
        
        public string? VIN { get; set; }

        public int quilometragem { get; set; }
    }
}
