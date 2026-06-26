namespace Versao1TrabalhoFinal.Models.PainelEmpresa
{
    /// <summary>
    /// Representa um cliente em destaque no painel.
    /// </summary>
    public class PainelTopClienteViewModel
    {
        /// <summary>
        /// Nome do cliente.
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Total gasto.
        /// </summary>
        public decimal TotalGasto { get; set; }

        /// <summary>
        /// Número de compras.
        /// </summary>
        public int TotalCompras { get; set; }

        /// <summary>
        /// Última data de compra.
        /// </summary>
        public DateTime? UltimaCompra { get; set; }
    }
}