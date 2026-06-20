using System.ComponentModel.DataAnnotations.Schema;

namespace Versao1TrabalhoFinal.Models
{
    /// <summary>
    /// Representa um item de uma venda.
    /// </summary>
    public class VendaItem
    {
        /// <summary>
        /// Identificador do item.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador da venda.
        /// </summary>
        public int VendaId { get; set; }

        /// <summary>
        /// Venda associada ao item.
        /// </summary>
        public Venda? Venda { get; set; }

        /// <summary>
        /// Identificador do produto.
        /// </summary>
        public int? ProdutoId { get; set; }

        /// <summary>
        /// Produto associado ao item.
        /// </summary>
        public Produto? Produto { get; set; }

        /// <summary>
        /// Identificador do serviço.
        /// </summary>
        public int? ServicoId { get; set; }

        /// <summary>
        /// Serviço associado ao item.
        /// </summary>
        public Servico? Servico { get; set; }

        /// <summary>
        /// Quantidade vendida.
        /// </summary>
        public int Quantidade { get; set; }

        /// <summary>
        /// Preço unitário.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoUnitario { get; set; }
    }
}