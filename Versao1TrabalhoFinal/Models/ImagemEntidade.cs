using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Versao1TrabalhoFinal.Models
{
    /// <summary>
    /// Representa uma imagem associada a uma entidade da aplicação.
    /// </summary>
    [Index(nameof(TipoEntidade), nameof(EntidadeId))]
    public class ImagemEntidade
    {
        /// <summary>
        /// Identificador único da imagem.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// URL da imagem.
        /// </summary>
        [Required]
        [StringLength(500)]
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Texto alternativo da imagem.
        /// </summary>
        [StringLength(200)]
        public string? Alt { get; set; }

        /// <summary>
        /// Ordem da imagem na galeria.
        /// </summary>
        public int Ordem { get; set; } = 0;

        /// <summary>
        /// Indica se a imagem é a principal.
        /// </summary>
        public bool Principal { get; set; } = false;

        /// <summary>
        /// Id da entidade associada.
        /// </summary>
        public int EntidadeId { get; set; }

        /// <summary>
        /// Tipo da entidade associada, por exemplo Produto, Servico ou VeiculoStand.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string TipoEntidade { get; set; } = string.Empty;
    }
}