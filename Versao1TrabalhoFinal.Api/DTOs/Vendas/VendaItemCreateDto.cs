using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Api.DTOs.Vendas
{
    /// <summary>
    /// DTO para criação de itens de venda.
    /// </summary>
    public class VendaItemCreateDto
    {
        public int? ProdutoId { get; set; }

        public int? ServicoId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantidade { get; set; }

        [Range(0, double.MaxValue)]
        public decimal PrecoUnitario { get; set; }
    }
}