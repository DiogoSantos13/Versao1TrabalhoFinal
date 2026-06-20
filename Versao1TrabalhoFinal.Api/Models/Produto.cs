using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Versao1TrabalhoFinal.Models
{
    /// <summary>
    /// Representa um produto disponível para venda na aplicação.
    /// </summary>
    public class Produto 
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
        public int? FornecedorId { get; set; }

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

       
        }
}