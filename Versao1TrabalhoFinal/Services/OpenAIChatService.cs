using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Versao1TrabalhoFinal.Models.Chat;

namespace Versao1TrabalhoFinal.Services
{
    public class OpenAIChatService : IOpenAIChatService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public OpenAIChatService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<string> GerarRespostaAsync(List<ChatMessage> messages)
        {
            var apiKey = _configuration["OpenAI:ApiKey"];
            var model = _configuration["OpenAI:Model"] ?? "gpt-4o-mini";

            if (string.IsNullOrWhiteSpace(apiKey))
                return "A chave da API OpenAI não está configurada.";

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var request = new ChatRequest
            {
                model = model,
                messages = messages,
                temperature = 0.4
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return $"Erro OpenAI: {responseBody}";

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var result = JsonSerializer.Deserialize<ChatResponse>(responseBody, options);

            return result?.Choices?.FirstOrDefault()?.Message?.Content?.Trim()
                   ?? "Não foi possível gerar resposta.";
        }
    }
}
