namespace Versao1TrabalhoFinal.Api.Middleware
{
    /// <summary>
    /// Métodos de extensão para registar o middleware de logging de pedidos.
    /// </summary>
    public static class RequestLoggingMiddlewareExtensions
    {
        /// <summary>
        /// Adiciona o middleware de logging de pedidos à pipeline da aplicação.
        /// </summary>
        /// <param name="app">Builder da aplicação.</param>
        /// <returns>Builder da aplicação.</returns>
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}