using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Models
{
    public class ChatConversationMessage
    {
        public int Id { get; set; }

        public int ChatConversationId { get; set; }
        public Chat.ChatConversation? ChatConversation { get; set; }

        [Required]
        public string Role { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime DataMensagem { get; set; } = DateTime.Now;
    }
}
