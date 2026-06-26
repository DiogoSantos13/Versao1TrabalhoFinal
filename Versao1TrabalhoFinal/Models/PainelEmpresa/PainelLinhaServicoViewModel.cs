namespace Versao1TrabalhoFinal.Models.PainelEmpresa
{
    /// <summary>
    /// Representa um serviço ou item operacional a apresentar no painel.
    /// </summary>
    public class PainelLinhaServicoViewModel
    {
        /// <summary>
        /// Descrição do serviço.
        /// </summary>
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Quantidade do item ou serviço.
        /// </summary>
        public int Quantidade { get; set; }

        /// <summary>
        /// Valor unitário.
        /// </summary>
        public decimal ValorUnitario { get; set; }

        /// <summary>
        /// Valor total da linha.
        /// </summary>
        public decimal ValorTotal => Quantidade * ValorUnitario;
    }
}