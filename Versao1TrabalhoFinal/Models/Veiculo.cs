namespace Versao1TrabalhoFinal.Models
{
    public class Veiculo
    {
        public int Id { get; set; }
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public int Ano { get; set; }
        public string? Tipo { get; set; }
        public int Cilindrada { get; set; }
        public string? Combustivel { get; set; }
        public string? Matricula { get; set; }
        public string? VIN { get; set; }
        public string? Cor { get; set; }
        public int ClienteId { get; set; }
        public string? ImagemUrl { get; set; }

        public Cliente? Cliente { get; set; }
        public ICollection<VeiculoStand> VeiculosStand { get; set; } = new List<VeiculoStand>();
        public ICollection<OrdemReparacao> OrdensReparacao { get; set; } = new List<OrdemReparacao>();
        public ICollection<Orcamento> Orcamentos { get; set; } = new List<Orcamento>();
        public ICollection<HistoricoReparacao> HistoricoReparacoes { get; set; } = new List<HistoricoReparacao>();
    }
}
