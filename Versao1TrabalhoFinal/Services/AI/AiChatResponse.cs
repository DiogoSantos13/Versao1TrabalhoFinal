namespace Versao1TrabalhoFinal.Services.AI
{
    public class AiChatResponse
    {
        public bool Success { get; set; }
        public string Reply { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
    }
}
