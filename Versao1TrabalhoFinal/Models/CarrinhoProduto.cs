using System.ComponentModel.DataAnnotations.Schema;

namespace Versao1TrabalhoFinal.Models
{
    /// <summary>
    /// Representa a ligação entre um carrinho e um produto.
    /// Permite guardar produtos no mesmo carrinho onde também existem veículos e serviços.
    /// </summary>
    public class CarrinhoProdutos
    {
        /// <summary>
        /// Identificador único do item de produto no carrinho.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador do carrinho.
        /// </summary>
        public int CarrinhoId { get; set; }

        /// <summary>
        /// Identificador do produto.
        /// </summary>
        public int ProdutoId { get; set; }

        /// <summary>
        /// Quantidade do produto no carrinho.
        /// </summary>
        public int Quantidade { get; set; } = 1;

        /// <summary>
        /// Preço do produto no momento em que foi adicionado ao carrinho.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoNoMomento { get; set; }

        /// <summary>
        /// Data em que o produto foi adicionado ao carrinho.
        /// </summary>
        public DateTime DataAdicao { get; set; } = DateTime.Now;

        /// <summary>
        /// Carrinho associado.
        /// </summary>
        public Carrinho Carrinho { get; set; } = null!;

        /// <summary>
        /// Produto associado.
        /// </summary>
        public Produto Produto { get; set; } = null!;
    }
}