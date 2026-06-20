using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Api.DTOs.Imagens
{
    /// <summary>
    /// DTO para criação de imagens associadas a entidades.
    /// </summary>
    public class ImagemEntidadeCreateDto
    {
        [Required]
        public string Entidade { get; set; } = string.Empty;

        [Required]
        public int EntidadeId { get; set; }

        [Required]
        public string Url { get; set; } = string.Empty;

        public string? Descricao { get; set; }
    }
}