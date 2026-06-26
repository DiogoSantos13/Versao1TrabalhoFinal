using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Versao1TrabalhoFinal.Models
{
    /// <summary>
    /// Representa um produto disponível para venda na aplicação.
    /// </summary>
    public class Produto : EntidadeComGaleria
    {
        /// <summary>
        /// Identificador único do produto.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome do produto.
        /// </summary>
        [Required]
        [StringLength(150)]
        public string? Nome { get; set; }

        /// <summary>
        /// Descrição detalhada do produto.
        /// </summary>
        [Required]
        [StringLength(1000)]
        public string? Descricao { get; set; }

        /// <summary>
        /// Preço atual do produto.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal Preco { get; set; }

        /// <summary>
        /// Quantidade disponível em stock.
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        /// Categoria a que o produto pertence (ex.: Óleo, Filtro, Travagem).
        /// </summary>
        [StringLength(100)]
        public string? Categoria { get; set; }

        /// <summary>
        /// Identificador do fornecedor associado ao produto.
        /// </summary>
        public int FornecedorId { get; set; }

        /// <summary>
        /// URL da imagem do produto.
        /// </summary>
        [StringLength(500)]
        public string? ImagemUrl { get; set; }

        /// <summary>
        /// Marca de veículo compatível com o produto.
        /// </summary>
        [StringLength(100)]
        public string? MarcaVeiculo { get; set; }

        /// <summary>
        /// Modelo de veículo compatível com o produto.
        /// </summary>
        [StringLength(100)]
        public string? ModeloVeiculo { get; set; }

        /// <summary>
        /// Fornecedor associado ao produto.
        /// </summary>
        public Fornecedor? Fornecedor { get; set; }

        /// <summary>
        /// Relação com produtos usados em ordens de reparação.
        /// </summary>
        public ICollection<OrdemProduto> OrdemProdutos { get; set; } = new List<OrdemProduto>();

        /// <summary>
        /// Relação com itens de orçamento que utilizam este produto.
        /// </summary>
        public ICollection<OrcamentoItem> OrcamentoItens { get; set; } = new List<OrcamentoItem>();

        /// <summary>
        /// Relação com itens de venda que utilizam este produto.
        /// </summary>
        public ICollection<VendaItem> VendaItens { get; set; } = new List<VendaItem>();

        /// <summary>
        /// Relação com itens de carrinho de produtos (tabela de ligação).
        /// </summary>
        public ICollection<CarrinhoProdutos> CarrinhoProdutos { get; set; } = new List<CarrinhoProdutos>();
    
    
    }
}