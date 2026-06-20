using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Api.DTOs.Common
{
    /// <summary>
    /// Representa os parâmetros de paginação recebidos pela query string.
    /// </summary>
    public class PaginationQueryDto
    {
        /// <summary>
        /// Número da página pretendida.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        /// <summary>
        /// Quantidade de registos por página.
        /// </summary>
        [Range(1, 100)]
        public int PageSize { get; set; } = 10;
    }
}