using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Api.DTOs.PedidosContacto
{
    /// <summary>
    /// DTO utilizado para atualizar um pedido de contacto existente.
    /// </summary>
    public class PedidoContactoUpdateDto
    {
        /// <summary>
        /// Identificador do pedido de contacto.
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Nome da pessoa que efetuou o pedido.
        /// </summary>
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome não pode ter mais de 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Endereço de email da pessoa que efetuou o pedido.
        /// </summary>
        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O email introduzido não é válido.")]
        [StringLength(150, ErrorMessage = "O email não pode ter mais de 150 caracteres.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Número de telefone da pessoa que efetuou o pedido.
        /// </summary>
        [StringLength(20, ErrorMessage = "O telefone não pode ter mais de 20 caracteres.")]
        public string? Telefone { get; set; }

        /// <summary>
        /// Assunto do pedido de contacto.
        /// </summary>
        [Required(ErrorMessage = "O assunto é obrigatório.")]
        [StringLength(150, ErrorMessage = "O assunto não pode ter mais de 150 caracteres.")]
        public string Assunto { get; set; } = string.Empty;

        /// <summary>
        /// Mensagem enviada no pedido de contacto.
        /// </summary>
        [Required(ErrorMessage = "A mensagem é obrigatória.")]
        [StringLength(2000, ErrorMessage = "A mensagem não pode ter mais de 2000 caracteres.")]
        public string Mensagem { get; set; } = string.Empty;

        /// <summary>
        /// Indica se o pedido já foi respondido.
        /// </summary>
        public bool Respondido { get; set; }
    }
}