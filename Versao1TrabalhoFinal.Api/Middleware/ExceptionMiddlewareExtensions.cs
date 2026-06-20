
namespace Versao1TrabalhoFinal.Api.Middleware
{
    /// <summary>
    /// Extensões para registar o middleware de tratamento global de exceções.
    /// </summary>
    public static class ExceptionMiddlewareExtensions
    {
        /// <summary>
        /// Adiciona o middleware de exceções à pipeline da aplicação.
        /// </summary>
        /// <param name="app">Builder da aplicação.</param>
        /// <returns>Builder da aplicação.</returns>
        public static IApplicationBuilder UseCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}