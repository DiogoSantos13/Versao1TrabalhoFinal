using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Api.DTOs.Clientes
{
    /// <summary>
    /// DTO para criação de clientes.
    /// </summary>
    public class ClienteCreateDto
    {
        [Required]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? NIF { get; set; }

        public string? Telefone { get; set; }

        public string? Morada { get; set; }

        public string? IdentityUserId { get; set; }
    }
}