namespace Versao1TrabalhoFinal.Models
{
    /// <summary>
    /// Representa o carrinho único do cliente.
    /// </summary>
    public class Carrinho
    {
        /// <summary>
        /// Identificador do carrinho.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador do cliente associado ao carrinho.
        /// </summary>
        public int ClienteId { get; set; }

        /// <summary>
        /// Data de criação do carrinho.
        /// </summary>
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        /// <summary>
        /// Cliente associado ao carrinho.
        /// </summary>
        public Cliente Cliente { get; set; } = null!;


        /// <summary>
        /// Serviços adicionados ao carrinho.
        /// </summary>
        public ICollection<CarrinhoServico> Servicos { get; set; } = new List<CarrinhoServico>();

        /// <summary>
        /// Produtos adicionados ao carrinho.
        /// </summary>
        public ICollection<CarrinhoProdutos> Produtos { get; set; } = new List<CarrinhoProdutos>();
    }
}