using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Models
{
    /// <summary>
    /// Representa uma imagem associada de forma genérica a uma entidade do sistema.
    /// </summary>
    public class ImagemEntidade
    {
        /// <summary>
        /// Identificador da imagem.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome da entidade associada à imagem.
        /// </summary>
        [Required(ErrorMessage = "O nome da entidade é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome da entidade não pode ter mais de 100 caracteres.")]
        public string NomeEntidade { get; set; } = string.Empty;

        /// <summary>
        /// Identificador do registo da entidade associada.
        /// </summary>
        [Required(ErrorMessage = "O identificador da entidade é obrigatório.")]
        public int EntidadeId { get; set; }

        /// <summary>
        /// Caminho ou URL da imagem.
        /// </summary>
        [Required(ErrorMessage = "O caminho da imagem é obrigatório.")]
        [StringLength(500, ErrorMessage = "O caminho da imagem não pode ter mais de 500 caracteres.")]
        public string Caminho { get; set; } = string.Empty;

        /// <summary>
        /// Texto alternativo da imagem.
        /// </summary>
        [StringLength(200, ErrorMessage = "O texto alternativo não pode ter mais de 200 caracteres.")]
        public string? AltText { get; set; }

        /// <summary>
        /// Indica se a imagem é a principal.
        /// </summary>
        public bool Principal { get; set; }

        /// <summary>
        /// Data de criação do registo.
        /// </summary>
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    }
}