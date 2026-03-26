using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Versao1TrabalhoFinal.Extensions
{
    /// <summary>
    /// Métodos de extensão para guardar e obter objetos complexos na sessão.
    /// </summary>
    public static class SessionExtensions
    {
        /// <summary>
        /// Guarda um objeto na sessão em formato JSON.
        /// </summary>
        /// <typeparam name="T">Tipo do objeto.</typeparam>
        /// <param name="session">Sessão HTTP.</param>
        /// <param name="key">Chave de armazenamento.</param>
        /// <param name="value">Valor a guardar.</param>
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            var json = JsonSerializer.Serialize(value);
            session.SetString(key, json);
        }

        /// <summary>
        /// Obtém um objeto da sessão a partir de JSON.
        /// </summary>
        /// <typeparam name="T">Tipo do objeto.</typeparam>
        /// <param name="session">Sessão HTTP.</param>
        /// <param name="key">Chave de armazenamento.</param>
        /// <returns>Objeto desserializado ou valor por defeito.</returns>
        public static T? GetObject<T>(this ISession session, string key)
        {
            var json = session.GetString(key);

            if (string.IsNullOrWhiteSpace(json))
                return default;

            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
