using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Api.DTOs.Clientes
{
    /// <summary>
    /// DTO para atualização de clientes.
    /// </summary>
    public class ClienteUpdateDto
    {
        [Required]
        public int Id { get; set; }

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