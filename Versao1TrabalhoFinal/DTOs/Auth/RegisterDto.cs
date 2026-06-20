using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Cliente.DTOs.Auth
{
    /// <summary>
    /// DTO utilizado para enviar os dados de registo para a API.
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// Email do novo utilizador.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Palavra-passe do novo utilizador.
        /// </summary>
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }
}