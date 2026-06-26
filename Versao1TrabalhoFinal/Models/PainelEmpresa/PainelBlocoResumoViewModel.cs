namespace Versao1TrabalhoFinal.Models.PainelEmpresa
{
    /// <summary>
    /// Representa um pequeno bloco de resumo para uso no painel.
    /// </summary>
    public class PainelBlocoResumoViewModel
    {
        /// <summary>
        /// Título do bloco.
        /// </summary>
        public string Titulo { get; set; } = string.Empty;

        /// <summary>
        /// Valor principal.
        /// </summary>
        public string Valor { get; set; } = string.Empty;

        /// <summary>
        /// Descrição complementar.
        /// </summary>
        public string? Descricao { get; set; }
    }
}