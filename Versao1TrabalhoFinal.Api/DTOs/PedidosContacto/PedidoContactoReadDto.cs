namespace Versao1TrabalhoFinal.Api.DTOs.PedidosContacto
{
    /// <summary>
    /// DTO de leitura de pedidos de contacto.
    /// </summary>
    public class PedidoContactoReadDto
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Telefone { get; set; }

        public string Mensagem { get; set; } = string.Empty;

        public DateTime DataCriacao { get; set; }
    }
}