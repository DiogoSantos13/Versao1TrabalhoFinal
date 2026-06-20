namespace Versao1TrabalhoFinal.Api.DTOs.Veiculos
{
    /// <summary>
    /// DTO de leitura de veículos.
    /// </summary>
    public class VeiculoReadDto
    {
        public int Id { get; set; }

        public string Marca { get; set; } = string.Empty;

        public string Modelo { get; set; } = string.Empty;

        public string Matricula { get; set; } = string.Empty;

        public int Ano { get; set; }

        public string? Cor { get; set; }

        public string? Combustivel { get; set; }

        public int Quilometros { get; set; }

        public int? ClienteId { get; set; }

        public string? ClienteNome { get; set; }
    }
}