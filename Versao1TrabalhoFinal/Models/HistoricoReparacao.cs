

namespace Versao1TrabalhoFinal.Models
{
    public class HistoricoReparacao
    {
        public int Id { get; set; }
        public int VeiculoId { get; set; }
        public string? Problema { get; set; }
        public string? Solucao { get; set; }
        public decimal Custo { get; set; }
        public int TempoHoras { get; set; }
        public DateTime DataReparacao { get; set; }
        public Veiculo? Veiculo { get; set; }
    }
}
