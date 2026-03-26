using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;
using Versao1TrabalhoFinal.Models.Chat;
using Versao1TrabalhoFinal.Services.AI;

namespace Versao1TrabalhoFinal.Pages.Assistente
{
    /// <summary>
    /// Página do assistente virtual com histórico de conversa.
    /// </summary>
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IAiChatService _aiChatService;
        private readonly IChatConversationService _conversationService;
        private readonly StandDbContext _context;

        /// <summary>
        /// Inicializa a página do assistente.
        /// </summary>
        /// <param name="aiChatService">Serviço de comunicação com a IA.</param>
        /// <param name="conversationService">Serviço de gestão da conversa.</param>
        /// <param name="context">Contexto da base de dados.</param>
        public IndexModel(
            IAiChatService aiChatService,
            IChatConversationService conversationService,
            StandDbContext context)
        {
            _aiChatService = aiChatService;
            _conversationService = conversationService;
            _context = context;
        }

        /// <summary>
        /// Dados do formulário de envio da mensagem.
        /// </summary>
        [BindProperty]
        public ChatPromptInput Input { get; set; } = new();

        /// <summary>
        /// Conversa atual apresentada na interface.
        /// </summary>
        public ChatConversation Conversation { get; set; } = new();

        /// <summary>
        /// Mensagem de erro a mostrar na página.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Identificador do cliente para criar orçamento.
        /// </summary>
        [BindProperty]
        public int ClienteId { get; set; }

        /// <summary>
        /// Identificador do veículo para criar orçamento.
        /// </summary>
        [BindProperty]
        public int VeiculoId { get; set; }

        /// <summary>
        /// Lista de clientes disponível no formulário.
        /// </summary>
        public SelectList Clientes { get; set; } = default!;

        /// <summary>
        /// Lista de veículos disponível no formulário.
        /// </summary>
        public SelectList Veiculos { get; set; } = default!;

        /// <summary>
        /// Carrega a conversa atual e as listas auxiliares.
        /// </summary>
        public async Task OnGetAsync()
        {
            Conversation = _conversationService.GetConversation();
            await CarregarListasAsync();
        }

        /// <summary>
        /// Envia uma nova mensagem para a IA e atualiza a conversa.
        /// </summary>
        /// <returns>Página atualizada com a nova resposta.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            Conversation = _conversationService.GetConversation();
            await CarregarListasAsync();

            if (!ModelState.IsValid)
                return Page();

            _conversationService.AddMessage(ChatRole.User, Input.Prompt);

            var updatedConversation = _conversationService.GetConversation();

            var request = new AiChatRequest
            {
                UserMessage = Input.Prompt,
                History = updatedConversation.Messages
                    .Where(m => m.Role == ChatRole.User || m.Role == ChatRole.Assistant)
                    .SkipLast(1)
                    .ToList()
            };

            var result = await _aiChatService.AskAsync(request);

            if (!result.Success)
            {
                ErrorMessage = result.ErrorMessage ?? "Ocorreu um erro ao contactar a IA.";
                Conversation = _conversationService.GetConversation();
                return Page();
            }

            _conversationService.AddMessage(ChatRole.Assistant, result.Reply);
            Conversation = _conversationService.GetConversation();

            Input = new ChatPromptInput();
            ModelState.Clear();

            return Page();
        }

        /// <summary>
        /// Cria um orçamento a partir da conversa atual com a IA.
        /// </summary>
        /// <returns>Redireciona para o detalhe do orçamento criado.</returns>
        public async Task<IActionResult> OnPostCriarOrcamentoAsync()
        {
            Conversation = _conversationService.GetConversation();
            await CarregarListasAsync();

            if (ClienteId <= 0)
                ModelState.AddModelError(string.Empty, "Seleciona um cliente.");

            if (VeiculoId <= 0)
                ModelState.AddModelError(string.Empty, "Seleciona um veículo.");

            if (!Conversation.Messages.Any(m => m.Role == ChatRole.User))
                ModelState.AddModelError(string.Empty, "Ainda não existe conversa suficiente para gerar orçamento.");

            if (!ModelState.IsValid)
                return Page();

            var ultimaPergunta = Conversation.Messages
                .LastOrDefault(m => m.Role == ChatRole.User)?.Content ?? "Pedido criado a partir do chat.";

            var ultimaResposta = Conversation.Messages
                .LastOrDefault(m => m.Role == ChatRole.Assistant)?.Content ?? string.Empty;

            var descricaoFinal =
                $"Pedido do cliente: {ultimaPergunta}{Environment.NewLine}{Environment.NewLine}Resposta IA: {ultimaResposta}";

            var orcamento = new Orcamento
            {
                ClienteId = ClienteId,
                VeiculoId = VeiculoId,
                Descricao = descricaoFinal,
                ValorEstimado = 0,
                //GeradoComAI = true,
                //DataPedido = DateTime.Now,
                Estado = EstadoOrcamento.Pendente
            };

            _context.Orcamentos.Add(orcamento);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Orcamentos/Details", new { id = orcamento.Id });
        }

        /// <summary>
        /// Limpa o histórico da conversa atual.
        /// </summary>
        /// <returns>Redireciona para a mesma página sem mensagens.</returns>
        public IActionResult OnPostClear()
        {
            _conversationService.ClearConversation();
            return RedirectToPage();
        }

        /// <summary>
        /// Carrega listas auxiliares de clientes e veículos.
        /// </summary>
        private async Task CarregarListasAsync()
        {
            var clientes = await _context.Clientes
                .OrderBy(c => c.Nome)
                .ToListAsync();

            var veiculos = await _context.Veiculos
                .OrderBy(v => v.Matricula)
                .ToListAsync();

            Clientes = new SelectList(clientes, "Id", "Nome");
            Veiculos = new SelectList(veiculos, "Id", "Matricula");
        }
    }
}
