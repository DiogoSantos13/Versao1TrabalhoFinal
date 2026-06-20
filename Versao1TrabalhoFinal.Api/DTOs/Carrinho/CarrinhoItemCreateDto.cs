using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Api.DTOs.Carrinho
{
    /// <summary>
    /// DTO para criação de itens do carrinho.
    /// </summary>
    public class CarrinhoItemCreateDto
    {
        [Required]
        public int VeiculoId { get; set; }

        public int? ProdutoId { get; set; }

        public int? ServicoId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantidade { get; set; }

        [Range(0, double.MaxValue)]
        public decimal PrecoUnitario { get; set; }
    }
}