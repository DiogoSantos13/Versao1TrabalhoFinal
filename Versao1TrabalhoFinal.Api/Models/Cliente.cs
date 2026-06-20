using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Telefone { get; set; }

        [StringLength(20)]
        public string? NIF { get; set; }

        [StringLength(200)]
        public string? Morada { get; set; }

        [StringLength(250)]
        public string? ImagemUrl { get; set; }


        [Required]
        public string? IdentityUserId { get; set; } = string.Empty;

        //public DateTime DataCriacao { get; set; } = DateTime.Now;
        public ICollection<Veiculo> Veiculos { get; set; } = new List<Veiculo>();

     


    }
}
