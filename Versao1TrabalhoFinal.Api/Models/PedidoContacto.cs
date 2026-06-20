using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Models
{
    /// <summary>
    /// Representa um pedido de contacto enviado por um utilizador.
    /// </summary>
    public class PedidoContacto
    {
        /// <summary>
        /// Identificador do pedido.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome da pessoa que enviou o pedido.
        /// </summary>
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome não pode ter mais de 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Email da pessoa.
        /// </summary>
        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O email não é válido.")]
        [StringLength(150, ErrorMessage = "O email não pode ter mais de 150 caracteres.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Telefone de contacto.
        /// </summary>
        [StringLength(20, ErrorMessage = "O telefone não pode ter mais de 20 caracteres.")]
        public string? Telefone { get; set; }

        /// <summary>
        /// Mensagem enviada.
        /// </summary>
        [Required(ErrorMessage = "A mensagem é obrigatória.")]
        [StringLength(1000, ErrorMessage = "A mensagem não pode ter mais de 1000 caracteres.")]
        public string Mensagem { get; set; } = string.Empty;

        /// <summary>
        /// Data de criação do pedido.
        /// </summary>
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    }
}