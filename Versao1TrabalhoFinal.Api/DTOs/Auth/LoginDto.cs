using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Api.DTOs.Auth
{
    /// <summary>
    /// DTO para autenticação de utilizadores.
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Email do utilizador.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Palavra-passe do utilizador.
        /// </summary>
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}