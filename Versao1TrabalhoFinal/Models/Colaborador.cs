using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Models
{
    /// <summary>
    /// Entidade que representa um colaborador da empresa.
    /// Os dados de autenticação e permissões ficam no ASP.NET Identity.
    /// </summary>
    public class Colaborador
    {
        /// <summary>
        /// Identificador interno do colaborador.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador do utilizador do ASP.NET Identity associado.
        /// </summary>
        [Required]
        [StringLength(450)]
        public string IdentityUserId { get; set; } = string.Empty;

        /// <summary>
        /// Nome do colaborador.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Telefone do colaborador.
        /// </summary>
        [Phone]
        [StringLength(20)]
        public string? Telefone { get; set; }

        /// <summary>
        /// Email do colaborador.
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// NIF do colaborador.
        /// </summary>
        [StringLength(20)]
        public string? NIF { get; set; }

        /// <summary>
        /// Morada do colaborador.
        /// </summary>
        [StringLength(250)]
        public string? Morada { get; set; }

        /// <summary>
        /// URL da imagem do colaborador.
        /// </summary>
        [StringLength(500)]
        public string? ImagemUrl { get; set; }

        /// <summary>
        /// Cargo interno ou designação livre.
        /// </summary>
        [StringLength(100)]
        public string? Cargo { get; set; }

        /// <summary>
        /// Data de criação do registo.
        /// </summary>
        public DateTime DataCriacao { get; set; } = DateTime.Now;
    }
}