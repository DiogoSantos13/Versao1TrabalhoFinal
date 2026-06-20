namespace Versao1TrabalhoFinal.Api.Models
{
    /// <summary>
    /// Representa as definições JWT da aplicação.
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// Chave secreta usada para assinar o token.
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// Emissor do token.
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// Audiência do token.
        /// </summary>
        public string Audience { get; set; } = string.Empty;

        /// <summary>
        /// Tempo de expiração em minutos.
        /// </summary>
        public int ExpirationMinutes { get; set; } = 180;
    }
}