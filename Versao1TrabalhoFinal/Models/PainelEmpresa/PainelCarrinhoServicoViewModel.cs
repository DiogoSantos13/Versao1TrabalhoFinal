namespace Versao1TrabalhoFinal.Models.PainelEmpresa
{
    /// <summary>
    /// Representa um serviço presente no carrinho de um cliente.
    /// </summary>
    public class PainelCarrinhoServicoViewModel
    {
        /// <summary>
        /// Nome do serviço.
        /// </summary>
        public string NomeServico { get; set; } = string.Empty;

        /// <summary>
        /// Preço guardado no momento da adição.
        /// </summary>
        public decimal PrecoNoMomento { get; set; }

        /// <summary>
        /// Data em que o serviço foi adicionado ao carrinho.
        /// </summary>
        public DateTime DataAdicao { get; set; }
    }
}