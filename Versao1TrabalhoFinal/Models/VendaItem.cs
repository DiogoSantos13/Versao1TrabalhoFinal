
namespace Versao1TrabalhoFinal.Models
{
    public class VendaItem
    {
        public int Id { get; set; }
        public int VendaId { get; set; }
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal Preco { get; set; }

        public Venda? Venda { get; set; }
        public Produto? Produto { get; set; }
    }
}
