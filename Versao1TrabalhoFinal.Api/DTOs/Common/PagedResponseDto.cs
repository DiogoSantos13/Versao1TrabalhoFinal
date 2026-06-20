namespace Versao1TrabalhoFinal.Api.DTOs.Common
{
    /// <summary>
    /// Representa uma resposta paginada da API.
    /// </summary>
    /// <typeparam name="T">Tipo dos elementos devolvidos.</typeparam>
    public class PagedResponseDto<T>
    {
        /// <summary>
        /// Lista de registos da página atual.
        /// </summary>
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

        /// <summary>
        /// Número total de registos encontrados.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Página atual.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Quantidade de registos por página.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Número total de páginas.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Indica se existe página anterior.
        /// </summary>
        public bool HasPreviousPage => Page > 1;

        /// <summary>
        /// Indica se existe página seguinte.
        /// </summary>
        public bool HasNextPage => Page < TotalPages;
    }
}