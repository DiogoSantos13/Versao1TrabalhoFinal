
namespace Versao1TrabalhoFinal.Models
{
    public class Orcamento
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int VeiculoId { get; set; }
        public string? Descricao { get; set; }
        public decimal ValorEstimado { get; set; }
        public bool GeradoPorIA { get; set; }
        public DateTime DataCriacao { get; set; }
        public string? Estado { get; set; }

        public Cliente? Cliente { get; set; }
        public Veiculo? Veiculo { get; set; }
        public ICollection<OrcamentoItem> Itens { get; set; } = new List<OrcamentoItem>();
    }
}
