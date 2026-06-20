using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Api.DTOs.Fornecedores
{
    /// <summary>
    /// DTO para criação de fornecedores.
    /// </summary>
    public class FornecedorCreateDto
    {
        [Required]
        public string Nome { get; set; } = string.Empty;

        [EmailAddress]
        public string? Email { get; set; }

        public string? Telefone { get; set; }

        public string? Morada { get; set; }
    }
}