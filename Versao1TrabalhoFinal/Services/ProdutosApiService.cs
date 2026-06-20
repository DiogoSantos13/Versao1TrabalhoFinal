using System.Net.Http.Headers;
using System.Net.Http.Json;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Cliente.Services
{
    /// <summary>
    /// Serviço responsável pelas chamadas ao endpoint de produtos da API.
    /// </summary>
    public class ProdutosApiService
    {
        /// <summary>
        /// Cliente HTTP usado para comunicar com a API.
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Serviço que fornece o token JWT atual.
        /// </summary>
        private readonly TokenService _tokenService;

        /// <summary>
        /// Inicializa uma nova instância do serviço de produtos.
        /// </summary>
        /// <param name="httpClient">Cliente HTTP configurado com a URL base da API.</param>
        /// <param name="tokenService">Serviço de gestão do token JWT.</param>
        public ProdutosApiService(HttpClient httpClient, TokenService tokenService)
        {
            _httpClient = httpClient;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Obtém a lista de produtos a partir da API.
        /// </summary>
        /// <returns>Lista de produtos ou lista vazia em caso de falha.</returns>
        public async Task<List<Produto>> GetProdutosAsync()
        {
            // Obtém o token guardado na sessão.
            var token = _tokenService.GetToken();

            // Se existir token, adiciona-o ao cabeçalho Authorization.
            if (!string.IsNullOrWhiteSpace(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            // Chama o endpoint GET /api/Produtos.
            var produtos = await _httpClient.GetFromJsonAsync<List<Produto>>("Produtos");

            // Se a resposta for nula, devolve uma lista vazia.
            return produtos ?? new List<Produto>();
        }
    }
}