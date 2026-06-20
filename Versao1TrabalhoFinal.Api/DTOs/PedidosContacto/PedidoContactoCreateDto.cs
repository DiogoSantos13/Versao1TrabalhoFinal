using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Api.DTOs.PedidosContacto
{
    /// <summary>
    /// DTO para criação de pedidos de contacto.
    /// </summary>
    public class PedidoContactoCreateDto
    {
        [Required]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? Telefone { get; set; }

        [Required]
        public string Mensagem { get; set; } = string.Empty;
    }
}