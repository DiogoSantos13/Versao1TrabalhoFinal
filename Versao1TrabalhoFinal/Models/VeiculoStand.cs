namespace Versao1TrabalhoFinal.Models
{
    public class VeiculoStand
    {
        public int Id { get; set; }
        public int VeiculoId { get; set; }
        public decimal Preco { get; set; }
        public string? Estado { get; set; }
        public int Quilometros { get; set; }
        public string? Descricao { get; set; }
        public DateTime DataEntrada { get; set; }
        public string? ImagemUrl { get; set; }

        public Veiculo? Veiculo { get; set; }
    }
}
