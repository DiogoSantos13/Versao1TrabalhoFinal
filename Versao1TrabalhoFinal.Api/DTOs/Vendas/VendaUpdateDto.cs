using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Api.DTOs.Vendas
{
    /// <summary>
    /// DTO para atualização de vendas.
    /// </summary>
    public class VendaUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int ClienteId { get; set; }

        public string? Observacoes { get; set; }
    }
}