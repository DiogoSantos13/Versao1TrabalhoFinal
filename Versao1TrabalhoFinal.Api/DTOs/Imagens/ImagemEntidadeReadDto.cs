namespace Versao1TrabalhoFinal.Api.DTOs.Imagens
{
    /// <summary>
    /// DTO de leitura de imagens associadas a entidades.
    /// </summary>
    public class ImagemEntidadeReadDto
    {
        public int Id { get; set; }

        public string Entidade { get; set; } = string.Empty;

        public int EntidadeId { get; set; }

        public string Url { get; set; } = string.Empty;

        public string? Descricao { get; set; }
    }
}