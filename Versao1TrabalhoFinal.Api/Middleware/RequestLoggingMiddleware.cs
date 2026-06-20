using System.Diagnostics;

namespace Versao1TrabalhoFinal.Api.Middleware
{
    /// <summary>
    /// Middleware responsável por registar informação básica sobre cada pedido HTTP.
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        /// <summary>
        /// Inicializa uma nova instância do middleware de logging de pedidos.
        /// </summary>
        /// <param name="next">Próximo middleware da pipeline.</param>
        /// <param name="logger">Logger da aplicação.</param>
        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Processa o pedido HTTP e regista duração, método, caminho e código de estado.
        /// </summary>
        /// <param name="context">Contexto HTTP atual.</param>
        /// <returns>Tarefa assíncrona.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            await _next(context);

            stopwatch.Stop();

            _logger.LogInformation(
                "HTTP {Method} {Path} respondeu {StatusCode} em {ElapsedMilliseconds} ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
    }
}