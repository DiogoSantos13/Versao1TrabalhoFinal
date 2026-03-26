using Versao1TrabalhoFinal.Models.Chat;

namespace Versao1TrabalhoFinal.Services
{
    public interface IOpenAIChatService
    {
        Task<string> GerarRespostaAsync(List<ChatMessage> messages);
    }
}
