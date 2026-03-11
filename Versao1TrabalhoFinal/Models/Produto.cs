

namespace Versao1TrabalhoFinal.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        public decimal Preco { get; set; }
        public int Stock { get; set; }
        public string? Categoria { get; set; }
        public int FornecedorId { get; set; }
        public string? ImagemUrl { get; set; }
        public string? MarcaVeiculo { get; set; }
        public string? ModeloVeiculo { get; set; }

        public Fornecedor? Fornecedor { get; set; }
        public ICollection<OrdemProduto> OrdemProdutos { get; set; } = new List<OrdemProduto>();
        public ICollection<OrcamentoItem> OrcamentoItens { get; set; } = new List<OrcamentoItem>();
        public ICollection<VendaItem> VendaItens { get; set; } = new List<VendaItem>();
    }
}
