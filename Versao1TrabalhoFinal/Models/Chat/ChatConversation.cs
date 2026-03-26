namespace Versao1TrabalhoFinal.Models.Chat
{
    public class ChatConversation
    {
        public List<ChatMessage> Messages { get; set; } = new();

        public void AddMessage(ChatRole role, string content)
        {
            Messages.Add(new ChatMessage
            {
                Role = role,
                Content = content
            });
        }

        public void AddSystemMessage(string content)
        {
            AddMessage(ChatRole.System, content);
        }

        public void AddUserMessage(string content)
        {
            AddMessage(ChatRole.User, content);
        }

        public void AddAssistantMessage(string content)
        {
            AddMessage(ChatRole.Assistant, content);
        }
    }
}
