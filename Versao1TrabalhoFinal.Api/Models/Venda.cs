using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Versao1TrabalhoFinal.Models
{
    /// <summary>
    /// Representa uma venda realizada a um cliente.
    /// </summary>
    public class Venda
    {
        /// <summary>
        /// Identificador da venda.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador do cliente.
        /// </summary>
        [Required]
        public int ClienteId { get; set; }

        /// <summary>
        /// Cliente associado à venda.
        /// </summary>
        public Cliente? Cliente { get; set; }

        /// <summary>
        /// Data da venda.
        /// </summary>
        public DateTime Data { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Valor total da venda.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        public string? Observacoes { get; set; }= string.Empty;
        /// <summary>
        /// Lista de itens da venda.
        /// </summary>
        public List<VendaItem> Itens { get; set; } = new();
    }
}