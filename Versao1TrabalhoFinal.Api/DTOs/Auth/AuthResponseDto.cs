namespace Versao1TrabalhoFinal.Api.DTOs.Auth
{
    /// <summary>
    /// DTO devolvido pela API após uma autenticação bem-sucedida.
    /// </summary>
    public class AuthResponseDto
    {
        /// <summary>
        /// Token JWT gerado para o utilizador autenticado.
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Email do utilizador autenticado.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Nome de utilizador associado à conta autenticada.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Data e hora de expiração do token.
        /// </summary>
        public DateTime Expiration { get; set; }
    }
}