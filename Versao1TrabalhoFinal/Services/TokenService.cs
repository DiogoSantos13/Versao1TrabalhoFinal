using Microsoft.AspNetCore.Http;

namespace Versao1TrabalhoFinal.Cliente.Services
{
    /// <summary>
    /// Serviço responsável por guardar e obter o token JWT na sessão do utilizador.
    /// </summary>
    public class TokenService
    {
        /// <summary>
        /// Nome da chave usada para guardar o token na sessão.
        /// </summary>
        private const string TokenSessionKey = "AuthToken";

        /// <summary>
        /// Context accessor para aceder à sessão HTTP atual.
        /// </summary>
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Inicializa uma nova instância do serviço de token.
        /// </summary>
        /// <param name="httpContextAccessor">Acesso ao contexto HTTP atual.</param>
        public TokenService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Guarda o token JWT na sessão atual.
        /// </summary>
        /// <param name="token">Token JWT a guardar.</param>
        public void SaveToken(string token)
        {
            _httpContextAccessor.HttpContext?.Session.SetString(TokenSessionKey, token);
        }

        /// <summary>
        /// Obtém o token JWT guardado na sessão.
        /// </summary>
        /// <returns>Token JWT ou string vazia se não existir.</returns>
        public string GetToken()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString(TokenSessionKey) ?? string.Empty;
        }

        /// <summary>
        /// Remove o token JWT da sessão atual.
        /// </summary>
        public void RemoveToken()
        {
            _httpContextAccessor.HttpContext?.Session.Remove(TokenSessionKey);
        }

        /// <summary>
        /// Verifica se existe token guardado na sessão.
        /// </summary>
        /// <returns>True se existir token; caso contrário, false.</returns>
        public bool HasToken()
        {
            return !string.IsNullOrWhiteSpace(GetToken());
        }
    }
}