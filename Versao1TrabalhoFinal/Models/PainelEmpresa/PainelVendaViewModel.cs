namespace Versao1TrabalhoFinal.Models.PainelEmpresa
{
    /// <summary>
    /// Representa uma venda finalizada para apresentação no painel.
    /// </summary>
    public class PainelVendaViewModel
    {
        /// <summary>
        /// Identificador da venda.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome do cliente.
        /// </summary>
        public string ClienteNome { get; set; } = string.Empty;

        /// <summary>
        /// Data da venda.
        /// </summary>
        public DateTime DataVenda { get; set; }

        /// <summary>
        /// Valor total.
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Tipo de venda.
        /// </summary>
        public string Tipo { get; set; } = string.Empty;

        /// <summary>
        /// Resumo textual dos itens comprados.
        /// </summary>
        public string ItensResumo { get; set; } = string.Empty;
    }
}