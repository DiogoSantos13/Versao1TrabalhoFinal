

namespace Versao1TrabalhoFinal.Models
{
    public class OrdemProduto
    {
        public int Id { get; set; }
        public int OrdemId { get; set; }
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal Preco { get; set; }

        public OrdemReparacao? OrdemReparacao { get; set; }
        public Produto? Produto { get; set; }
    }
}
