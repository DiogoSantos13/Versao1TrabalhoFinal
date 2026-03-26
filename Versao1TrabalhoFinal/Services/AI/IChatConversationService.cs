using Versao1TrabalhoFinal.Models.Chat;

namespace Versao1TrabalhoFinal.Services.AI
{
    public interface IChatConversationService
    {
        ChatConversation GetConversation();
        void AddMessage(ChatRole role, string content);
        void ClearConversation();
    }
}
