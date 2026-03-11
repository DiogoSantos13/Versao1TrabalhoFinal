

namespace Versao1TrabalhoFinal.Models
{
    public class OrdemReparacao
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int VeiculoId { get; set; }
        public DateTime DataEntrada { get; set; }
        public DateTime? DataSaida { get; set; }
        public string? Estado { get; set; }
        public string? DescricaoProblema { get; set; }
        public decimal Total { get; set; }

        public Cliente? Cliente { get; set; }
        public Veiculo? Veiculo { get; set; }
        public ICollection<OrdemProduto> OrdemProdutos { get; set; } = new List<OrdemProduto>();
    }
}
