namespace Versao1TrabalhoFinal.Api.DTOs.Fornecedores
{
    /// <summary>
    /// DTO de leitura de fornecedores.
    /// </summary>
    public class FornecedorReadDto
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? Telefone { get; set; }

        public string? Morada { get; set; }
    }
}