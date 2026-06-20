namespace Versao1TrabalhoFinal.Api.DTOs.Clientes
{
    /// <summary>
    /// DTO de leitura de clientes.
    /// </summary>
    public class ClienteReadDto
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Telefone { get; set; }

        public string? Nif { get; set; }

        public string? Morada { get; set; }

        public string? IdentityUserId { get; set; }
    }
}