using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Cliente.DTOs.Auth
{
    /// <summary>
    /// DTO utilizado para enviar os dados de login para a API.
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