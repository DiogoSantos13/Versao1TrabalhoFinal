using Versao1TrabalhoFinal.Models.Chat;

namespace Versao1TrabalhoFinal.Services.AI
{
    /// <summary>
    /// Serviço responsável por comunicar com a API de inteligência artificial.
    /// </summary>
    public interface IAiChatService
    {
        /// <summary>
        /// Envia uma mensagem e o histórico ao modelo de IA.
        /// </summary>
        /// <param name="request">Pedido com mensagem e histórico.</param>
        /// <returns>Resposta textual do assistente.</returns>
        Task<AiChatResponse> AskAsync(AiChatRequest request);
    }
}
