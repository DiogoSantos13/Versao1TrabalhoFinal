using System.ComponentModel.DataAnnotations.Schema;

namespace Versao1TrabalhoFinal.Models
{
    /// <summary>
    /// Representa um produto/peça associado a uma ordem de reparação.
    /// Cada registo guarda a quantidade e o preço unitário no momento da associação.
    /// </summary>
    public class OrdemProduto
    {
        /// <summary>
        /// Identificador único do registo.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// FK da ordem de reparação.
        /// </summary>
        public int OrdemId { get; set; }

        /// <summary>
        /// FK do produto associado.
        /// </summary>
        public int ProdutoId { get; set; }

        /// <summary>
        /// Quantidade usada nessa ordem.
        /// </summary>
        public int Quantidade { get; set; }

        /// <summary>
        /// Preço unitário do produto no momento em que foi associado à ordem.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal Preco { get; set; }

        /// <summary>
        /// Navegação para a ordem de reparação.
        /// </summary>
        public OrdemReparacao OrdemReparacao { get; set; } = null!;

        /// <summary>
        /// Navegação para o produto associado.
        /// </summary>
        public Produto Produto { get; set; } = null!;
    }
}