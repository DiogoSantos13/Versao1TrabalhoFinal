using System.ComponentModel.DataAnnotations.Schema;

namespace Versao1TrabalhoFinal.Models
{
    /// <summary>
    /// Representa um item de um orçamento.
    /// </summary>
    public class OrcamentoItem
    {
        /// <summary>
        /// Identificador do item.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador do orçamento.
        /// </summary>
        public int OrcamentoId { get; set; }

        /// <summary>
        /// Orçamento associado ao item.
        /// </summary>
        public Orcamento? Orcamento { get; set; }

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
        /// Quantidade do item.
        /// </summary>
        public int Quantidade { get; set; }

        /// <summary>
        /// Preço unitário do item.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoUnitario { get; set; }
    }
}