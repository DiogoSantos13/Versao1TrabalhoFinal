using System.Text.Json.Serialization;

namespace Versao1TrabalhoFinal.Models.Chat
{
    public class ChatResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("created")]
        public long Created { get; set; }

        [JsonPropertyName("model")]
        public string? Model { get; set; }

        [JsonPropertyName("choices")]
        public List<ChatChoice> Choices { get; set; } = new();

        [JsonPropertyName("usage")]
        public ChatUsage? Usage { get; set; }
    }
}
