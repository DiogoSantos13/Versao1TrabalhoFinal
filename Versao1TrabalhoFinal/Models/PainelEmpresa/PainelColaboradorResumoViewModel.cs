namespace Versao1TrabalhoFinal.Models.PainelEmpresa
{
    /// <summary>
    /// Representa um colaborador em formato resumido para dropdowns e listagens.
    /// </summary>
    public class PainelColaboradorResumoViewModel
    {
        /// <summary>
        /// Identificador do colaborador.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome do colaborador.
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Email do colaborador.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Cargo do colaborador.
        /// </summary>
        public string? Cargo { get; set; }

        /// <summary>
        /// Texto completo pronto a usar em listas.
        /// </summary>
        public string NomeComCargo => string.IsNullOrWhiteSpace(Cargo) ? Nome : $"{Nome} - {Cargo}";
    }
}