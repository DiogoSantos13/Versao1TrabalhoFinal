using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Versao1TrabalhoFinal.Api.Middleware
{
    /// <summary>
    /// Middleware responsável pelo tratamento global de exceções.
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        /// <summary>
        /// Inicializa uma nova instância do middleware de exceções.
        /// </summary>
        /// <param name="next">Próximo middleware da pipeline.</param>
        /// <param name="logger">Logger da aplicação.</param>
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Processa o pedido HTTP e captura exceções não tratadas.
        /// </summary>
        /// <param name="context">Contexto HTTP atual.</param>
        /// <returns>Tarefa assíncrona.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu uma exceção não tratada.");

                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Constrói e devolve uma resposta padronizada em caso de erro.
        /// </summary>
        /// <param name="context">Contexto HTTP atual.</param>
        /// <param name="exception">Exceção capturada.</param>
        /// <returns>Tarefa assíncrona.</returns>
        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var problem = new ProblemDetails
            {
                Title = "Ocorreu um erro interno no servidor.",
                Status = context.Response.StatusCode,
                Detail = exception.Message,
                Instance = context.Request.Path
            };

            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}