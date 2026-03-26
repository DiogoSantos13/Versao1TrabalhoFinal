using Versao1TrabalhoFinal.Models.Chat;

namespace Versao1TrabalhoFinal.Services.AI
{
    public class AiChatRequest
    {
        public string UserMessage { get; set; } = string.Empty;
        public List<ChatMessage> History { get; set; } = new();
    }
}
