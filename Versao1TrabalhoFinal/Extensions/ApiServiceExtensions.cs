using Versao1TrabalhoFinal.Cliente.Services;

namespace Versao1TrabalhoFinal.Cliente.Extensions
{
    /// <summary>
    /// Classe de extensão para registar os serviços de integração com a API.
    /// </summary>
    public static class ApiServiceExtensions
    {
        /// <summary>
        /// Regista os serviços necessários para comunicação com a API.
        /// </summary>
        /// <param name="services">Coleção de serviços da aplicação.</param>
        /// <param name="configuration">Configuração da aplicação.</param>
        /// <returns>Coleção de serviços atualizada.</returns>
        public static IServiceCollection AddApiIntegration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Regista a configuração da API a partir do appsettings.json.
            services.Configure<ApiSettings>(configuration.GetSection("ApiSettings"));

            // Regista o acesso ao contexto HTTP.
            services.AddHttpContextAccessor();

            // Regista o serviço de sessão/token.
            services.AddScoped<TokenService>();

            // Regista o HttpClient para autenticação.
            services.AddHttpClient<AuthApiService>(client =>
            {
                client.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"]!);
            });

            // Regista o HttpClient para produtos.
            services.AddHttpClient<ProdutosApiService>(client =>
            {
                client.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"]!);
            });

            return services;
        }
    }
}