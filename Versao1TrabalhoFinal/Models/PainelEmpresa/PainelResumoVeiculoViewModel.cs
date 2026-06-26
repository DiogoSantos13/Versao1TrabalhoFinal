namespace Versao1TrabalhoFinal.Models.PainelEmpresa
{
    /// <summary>
    /// Representa a informação resumida de um veículo no painel.
    /// </summary>
    public class PainelResumoVeiculoViewModel
    {
        /// <summary>
        /// Identificador do veículo.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Marca do veículo.
        /// </summary>
        public string Marca { get; set; } = string.Empty;

        /// <summary>
        /// Modelo do veículo.
        /// </summary>
        public string Modelo { get; set; } = string.Empty;

        /// <summary>
        /// Matrícula do veículo.
        /// </summary>
        public string Matricula { get; set; } = string.Empty;

        /// <summary>
        /// Descrição pronta a mostrar na interface.
        /// </summary>
        public string DescricaoCompleta => $"{Marca} {Modelo} - {Matricula}";
    }
}