namespace Versao1TrabalhoFinal.Models.PainelEmpresa
{
    /// <summary>
    /// Representa um evento recente para mostrar no painel.
    /// </summary>
    public class PainelAtividadeRecenteViewModel
    {
        /// <summary>
        /// Tipo do evento.
        /// Exemplo: Venda, Orçamento, Ordem.
        /// </summary>
        public string Tipo { get; set; } = string.Empty;

        /// <summary>
        /// Título principal do evento.
        /// </summary>
        public string Titulo { get; set; } = string.Empty;

        /// <summary>
        /// Texto complementar do evento.
        /// </summary>
        public string? Descricao { get; set; }

        /// <summary>
        /// Data/hora do evento.
        /// </summary>
        public DateTime Data { get; set; }

        /// <summary>
        /// Classe CSS do badge.
        /// </summary>
        public string? ClasseCss { get; set; }
    }
}