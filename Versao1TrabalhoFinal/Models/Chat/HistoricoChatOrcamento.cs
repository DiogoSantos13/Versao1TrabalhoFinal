using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Models
{
    public class HistoricoChatOrcamento
    {
        public int Id { get; set; }

        [Required]
        public int OrcamentoId { get; set; }

        public Orcamento? Orcamento { get; set; }

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = string.Empty;

        [Required]
        [StringLength(5000)]
        public string Mensagem { get; set; } = string.Empty;

        public DateTime DataMensagem { get; set; } = DateTime.Now;
    }
}
