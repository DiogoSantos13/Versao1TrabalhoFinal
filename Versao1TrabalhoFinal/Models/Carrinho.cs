namespace Versao1TrabalhoFinal.Models
{
    /// <summary>
    /// Representa o carrinho de um cliente.
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
        /// Itens do carrinho.
        /// </summary>
        public ICollection<CarrinhoItem> Itens { get; set; } = new List<CarrinhoItem>();

        /// <summary>
        /// Serviços adicionados ao carrinho.
        /// </summary>
        public ICollection<CarrinhoServico> Servicos { get; set; } = new List<CarrinhoServico>();


    }
}
