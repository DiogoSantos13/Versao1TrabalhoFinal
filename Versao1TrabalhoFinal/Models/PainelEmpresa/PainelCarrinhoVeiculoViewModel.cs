namespace Versao1TrabalhoFinal.Models.PainelEmpresa
{
    /// <summary>
    /// Representa um veículo do stand presente no carrinho de um cliente.
    /// </summary>
    public class PainelCarrinhoVeiculoViewModel
    {
        /// <summary>
        /// Descrição amigável do veículo.
        /// </summary>
        public string VeiculoDescricao { get; set; } = string.Empty;

        /// <summary>
        /// Preço registado no momento da adição ao carrinho.
        /// </summary>
        public decimal PrecoNoMomento { get; set; }

        /// <summary>
        /// Data em que o veículo foi adicionado ao carrinho.
        /// </summary>
        public DateTime DataAdicao { get; set; }
    }
}