namespace Versao1TrabalhoFinal.Models.PainelEmpresa
{
    /// <summary>
    /// Representa um cartão KPI do painel.
    /// </summary>
    public class PainelKpiViewModel
    {
        /// <summary>
        /// Título do KPI.
        /// </summary>
        public string Titulo { get; set; } = string.Empty;

        /// <summary>
        /// Valor principal do KPI.
        /// </summary>
        public string Valor { get; set; } = string.Empty;

        /// <summary>
        /// Texto complementar ou contexto.
        /// </summary>
        public string? Subtitulo { get; set; }

        /// <summary>
        /// Classe CSS opcional para destacar visualmente o cartão.
        /// </summary>
        public string? ClasseCss { get; set; }

        /// <summary>
        /// Ícone Font Awesome opcional.
        /// </summary>
        public string? Icone { get; set; }
    }
}