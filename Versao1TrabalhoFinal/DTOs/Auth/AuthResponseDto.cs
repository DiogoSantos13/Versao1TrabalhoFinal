namespace Versao1TrabalhoFinal.Cliente.DTOs.Auth
{
    /// <summary>
    /// DTO devolvido pela API após autenticação com sucesso.
    /// </summary>
    public class AuthResponseDto
    {
        /// <summary>
        /// Token JWT devolvido pela API.
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Email do utilizador autenticado.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Nome do utilizador autenticado.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Data de expiração do token.
        /// </summary>
        public DateTime Expiration { get; set; }
    }
}