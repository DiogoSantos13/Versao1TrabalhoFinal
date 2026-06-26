namespace Versao1TrabalhoFinal.Models.PainelEmpresa
{
    /// <summary>
    /// Representa um estado com metadados visuais para tabelas do painel.
    /// </summary>
    public class PainelTabelaEstadoViewModel
    {
        /// <summary>
        /// Texto do estado.
        /// </summary>
        public string Estado { get; set; } = string.Empty;

        /// <summary>
        /// Classe CSS associada ao badge.
        /// </summary>
        public string ClasseCss { get; set; } = "status-neutral";

        /// <summary>
        /// Ícone opcional.
        /// </summary>
        public string? Icone { get; set; }
    }
}