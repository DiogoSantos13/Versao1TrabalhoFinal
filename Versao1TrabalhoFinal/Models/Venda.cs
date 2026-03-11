
namespace Versao1TrabalhoFinal.Models
{
    public class Venda
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public DateTime DataVenda { get; set; }
        public decimal Total { get; set; }
        public string? Tipo { get; set; }

        public Cliente? Cliente { get; set; }
        public ICollection<VendaItem> Itens { get; set; } = new List<VendaItem>();
    }
}
