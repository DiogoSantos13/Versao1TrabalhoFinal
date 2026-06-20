using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Api.DTOs.Auth
{
    /// <summary>
    /// DTO para registo de utilizadores.
    /// </summary>
    public class RegisterDto
    {
        
        /// <summary>
        /// Email do utilizador.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Palavra-passe.
        /// </summary>
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Role pretendida.
        /// </summary>
        public string Role { get; set; } = "Cliente";
    }
}