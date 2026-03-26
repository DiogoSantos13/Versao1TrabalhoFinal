using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Versao1TrabalhoFinal.Models.Chat;

namespace Versao1TrabalhoFinal.Services.AI
{
    public class AiChatService : IAiChatService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AiChatService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<AiChatResponse> AskAsync(AiChatRequest request)
        {
            try
            {
                var apiKey = _configuration["OpenAI:ApiKey"];
                var model = _configuration["OpenAI:Model"] ?? "gpt-4o-mini";

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    return new AiChatResponse
                    {
                        Success = false,
                        ErrorMessage = "A chave OpenAI não está configurada."
                    };
                }

                var messages = new List<object>
                {
                    new
                    {
                        role = "system",
                        content = "És um assistente virtual de oficina automóvel. Responde em português de Portugal, de forma clara e útil."
                    }
                };

                foreach (var msg in request.History)
                {
                    messages.Add(new
                    {
                        role = ConvertRole(msg.Role),
                        content = msg.Content
                    });
                }

                messages.Add(new
                {
                    role = "user",
                    content = request.UserMessage
                });

                var payload = new
                {
                    model,
                    messages,
                    temperature = 0.4
                };

                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", apiKey);

                var json = JsonSerializer.Serialize(payload);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", httpContent);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new AiChatResponse
                    {
                        Success = false,
                        ErrorMessage = responseBody
                    };
                }

                using var doc = JsonDocument.Parse(responseBody);
                var reply = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString() ?? "Sem resposta.";

                return new AiChatResponse
                {
                    Success = true,
                    Reply = reply
                };
            }
            catch (Exception ex)
            {
                return new AiChatResponse
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        private static string ConvertRole(ChatRole role)
        {
            return role switch
            {
                ChatRole.System => "system",
                ChatRole.User => "user",
                ChatRole.Assistant => "assistant",
                _ => "user"
            };
        }
    }
}
