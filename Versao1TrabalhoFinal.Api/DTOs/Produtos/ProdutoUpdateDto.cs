using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Api.DTOs.Produtos
{
    /// <summary>
    /// DTO para atualização de produtos.
    /// </summary>
    public class ProdutoUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;

        public string? Descricao { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Preco { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        public int? FornecedorId { get; set; }
    }
}