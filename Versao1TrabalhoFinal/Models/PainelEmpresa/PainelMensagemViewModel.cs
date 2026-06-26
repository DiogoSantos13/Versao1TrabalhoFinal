namespace Versao1TrabalhoFinal.Models.PainelEmpresa
{
    /// <summary>
    /// Representa uma mensagem visual do painel.
    /// </summary>
    public class PainelMensagemViewModel
    {
        /// <summary>
        /// Título da mensagem.
        /// </summary>
        public string Titulo { get; set; } = string.Empty;

        /// <summary>
        /// Conteúdo da mensagem.
        /// </summary>
        public string Conteudo { get; set; } = string.Empty;

        /// <summary>
        /// Tipo visual da mensagem.
        /// Exemplo: success, warning, error, info.
        /// </summary>
        public string Tipo { get; set; } = "info";
    }
}