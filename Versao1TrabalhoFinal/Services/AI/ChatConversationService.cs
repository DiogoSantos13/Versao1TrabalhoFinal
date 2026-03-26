using Microsoft.AspNetCore.Http;
using Versao1TrabalhoFinal.Extensions;
using Versao1TrabalhoFinal.Models.Chat;

namespace Versao1TrabalhoFinal.Services.AI
{
    public class ChatConversationService : IChatConversationService
    {
        private const string SessionKey = "AI_CHAT_CONVERSATION";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChatConversationService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ChatConversation GetConversation()
        {
            var session = _httpContextAccessor.HttpContext?.Session;

            if (session == null)
                return new ChatConversation();

            var conversation = session.GetObject<ChatConversation>(SessionKey);

            if (conversation == null)
            {
                conversation = new ChatConversation();
                session.SetObject(SessionKey, conversation);
            }

            return conversation;
        }

        public void AddMessage(ChatRole role, string content)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null)
                return;

            var conversation = GetConversation();

            conversation.Messages.Add(new ChatMessage
            {
                Role = role,
                Content = content
            });

            if (conversation.Messages.Count > 20)
            {
                conversation.Messages = conversation.Messages
                    .TakeLast(20)
                    .ToList();
            }

            session.SetObject(SessionKey, conversation);
        }

        public void ClearConversation()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            session?.Remove(SessionKey);
        }
    }
}
