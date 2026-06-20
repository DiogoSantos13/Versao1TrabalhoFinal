using System.Net.Http.Json;
using Versao1TrabalhoFinal.Cliente.DTOs.Auth;

namespace Versao1TrabalhoFinal.Cliente.Services
{
    /// <summary>
    /// Serviço responsável pelas chamadas de autenticação à API.
    /// </summary>
    public class AuthApiService
    {
        /// <summary>
        /// Cliente HTTP usado para comunicar com a API.
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Serviço de gestão de token na sessão.
        /// </summary>
        private readonly TokenService _tokenService;

        /// <summary>
        /// Inicializa uma nova instância do serviço de autenticação.
        /// </summary>
        /// <param name="httpClient">Cliente HTTP configurado com a URL base da API.</param>
        /// <param name="tokenService">Serviço de gestão do token JWT.</param>
        public AuthApiService(HttpClient httpClient, TokenService tokenService)
        {
            _httpClient = httpClient;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Envia um pedido de login para a API.
        /// </summary>
        /// <param name="dto">Dados de login do utilizador.</param>
        /// <returns>Resposta de autenticação devolvida pela API ou null em caso de falha.</returns>
        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            // Envia os dados de login para o endpoint da API.
            var response = await _httpClient.PostAsJsonAsync("Auth/login", dto);

            // Se a resposta não for bem-sucedida, devolve null.
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            // Lê o conteúdo JSON devolvido pela API.
            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

            // Se a resposta contiver token, guarda-o na sessão.
            if (authResponse != null && !string.IsNullOrWhiteSpace(authResponse.Token))
            {
                _tokenService.SaveToken(authResponse.Token);
            }

            return authResponse;
        }

        /// <summary>
        /// Envia um pedido de registo para a API.
        /// </summary>
        /// <param name="dto">Dados de registo do utilizador.</param>
        /// <returns>True se o registo for bem-sucedido; caso contrário, false.</returns>
        public async Task<bool> RegisterAsync(RegisterDto dto)
        {
            // Envia os dados de registo para a API.
            var response = await _httpClient.PostAsJsonAsync("Auth/register", dto);

            // Indica se o registo foi concluído com sucesso.
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Termina a sessão local do utilizador removendo o token.
        /// </summary>
        public void Logout()
        {
            _tokenService.RemoveToken();
        }
    }
}