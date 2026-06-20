using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Api.DTOs.Vendas
{
    /// <summary>
    /// DTO para criação de vendas.
    /// </summary>
    public class VendaCreateDto
    {
        [Required]
        public int ClienteId { get; set; }

        public string? Observacoes { get; set; }

        public List<VendaItemCreateDto> Itens { get; set; } = new();
    }
}