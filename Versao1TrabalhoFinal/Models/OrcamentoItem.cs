
namespace Versao1TrabalhoFinal.Models
{
    public class OrcamentoItem
    {
        public int Id { get; set; }
        public int OrcamentoId { get; set; }
        public int? ServicoId { get; set; }
        public int? ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal Preco { get; set; }

        public Orcamento? Orcamento { get; set; }
        public Servico? Servico { get; set; }
        public Produto? Produto { get; set; }
    }

}
