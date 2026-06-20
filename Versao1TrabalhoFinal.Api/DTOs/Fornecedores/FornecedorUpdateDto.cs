using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Api.DTOs.Fornecedores
{
    /// <summary>
    /// DTO para atualização de fornecedores.
    /// </summary>
    public class FornecedorUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;

        [EmailAddress]
        public string? Email { get; set; }

        public string? Telefone { get; set; }

        public string? Morada { get; set; }
    }
}