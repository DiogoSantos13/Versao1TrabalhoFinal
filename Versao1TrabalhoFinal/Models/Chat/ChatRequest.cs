namespace Versao1TrabalhoFinal.Models.Chat
{
    public class ChatRequest
    {
        public string model { get; set; } = "gpt-4o-mini";
        public List<ChatMessage> messages { get; set; } = new();
        public double temperature { get; set; } = 0.4;
    }
}
