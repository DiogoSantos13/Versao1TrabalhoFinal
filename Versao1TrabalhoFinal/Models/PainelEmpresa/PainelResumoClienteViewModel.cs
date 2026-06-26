namespace Versao1TrabalhoFinal.Models.PainelEmpresa
{
    /// <summary>
    /// Representa a informação resumida de um cliente no painel.
    /// </summary>
    public class PainelResumoClienteViewModel
    {
        /// <summary>
        /// Identificador do cliente.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome do cliente.
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Email do cliente.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Telefone do cliente.
        /// </summary>
        public string? Telefone { get; set; }

        /// <summary>
        /// Total gasto pelo cliente.
        /// </summary>
        public decimal TotalGasto { get; set; }

        /// <summary>
        /// Número de vendas associadas ao cliente.
        /// </summary>
        public int TotalCompras { get; set; }
    }
}