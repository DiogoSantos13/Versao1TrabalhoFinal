namespace Versao1TrabalhoFinal.Models.PainelEmpresa
{
    /// <summary>
    /// Representa uma linha resumida de item vendido no painel.
    /// </summary>
    public class PainelResumoVendaItemViewModel
    {
        /// <summary>
        /// Nome do item vendido.
        /// </summary>
        public string NomeItem { get; set; } = string.Empty;

        /// <summary>
        /// Quantidade vendida.
        /// </summary>
        public int Quantidade { get; set; }

        /// <summary>
        /// Preço unitário.
        /// </summary>
        public decimal PrecoUnitario { get; set; }

        /// <summary>
        /// Total da linha.
        /// </summary>
        public decimal TotalLinha => Quantidade * PrecoUnitario;
    }
}