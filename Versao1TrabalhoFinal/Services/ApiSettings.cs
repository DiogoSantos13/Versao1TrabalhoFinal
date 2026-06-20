namespace Versao1TrabalhoFinal.Cliente.Services
{
    /// <summary>
    /// Classe de configuração utilizada para guardar a URL base da API.
    /// </summary>
    public class ApiSettings
    {
        /// <summary>
        /// URL base da API.
        /// Exemplo: http://localhost:5018/api/
        /// </summary>
        public string BaseUrl { get; set; } = string.Empty;
    }
}