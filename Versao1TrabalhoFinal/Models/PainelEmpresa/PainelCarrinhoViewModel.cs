namespace Versao1TrabalhoFinal.Models.PainelEmpresa
{
    /// <summary>
    /// Representa um carrinho de cliente para consulta no painel da empresa.
    /// </summary>
    public class PainelCarrinhoViewModel
    {
        /// <summary>
        /// Identificador do carrinho.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador do cliente dono do carrinho.
        /// </summary>
        public int ClienteId { get; set; }

        /// <summary>
        /// Nome do cliente.
        /// </summary>
        public string ClienteNome { get; set; } = string.Empty;

        /// <summary>
        /// Data de criação do carrinho.
        /// </summary>
        public DateTime DataCriacao { get; set; }

        /// <summary>
        /// Produtos atualmente presentes no carrinho.
        /// </summary>
        public List<PainelCarrinhoProdutoViewModel> Produtos { get; set; } = new();

        /// <summary>
        /// Serviços atualmente presentes no carrinho.
        /// </summary>
        public List<PainelCarrinhoServicoViewModel> Servicos { get; set; } = new();

        /// <summary>
        /// Veículos do stand atualmente presentes no carrinho.
        /// </summary>
        public List<PainelCarrinhoVeiculoViewModel> Veiculos { get; set; } = new();
    }
}