namespace Versao1TrabalhoFinal.Api.DTOs.VeiculosStand
{
    /// <summary>
    /// DTO de leitura de veículos do stand.
    /// </summary>
    public class VeiculoStandReadDto
    {
        /// <summary>
        /// Identificador do registo do stand.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador do veículo associado.
        /// </summary>
        public int VeiculoId { get; set; }

        /// <summary>
        /// Marca do veículo.
        /// </summary>
        public string? Marca { get; set; }

        /// <summary>
        /// Modelo do veículo.
        /// </summary>
        public string? Modelo { get; set; }

        /// <summary>
        /// Matrícula do veículo.
        /// </summary>
        public string? Matricula { get; set; }

        /// <summary>
        /// Preço do veículo.
        /// </summary>
        public decimal Preco { get; set; }

        /// <summary>
        /// Estado atual do veículo no stand.
        /// </summary>
        public string Estado { get; set; } = string.Empty;
    }
}