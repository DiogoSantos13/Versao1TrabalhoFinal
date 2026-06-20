namespace Versao1TrabalhoFinal.Api.DTOs.Servicos
{
    /// <summary>
    /// DTO de leitura de serviços.
    /// </summary>
    public class ServicoReadDto
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string? Descricao { get; set; }

        public decimal PrecoBase { get; set; }

        public int? DuracaoEstimadaMinutos { get; set; }
    }
}