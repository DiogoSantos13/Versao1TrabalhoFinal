using System.ComponentModel.DataAnnotations.Schema;

namespace Versao1TrabalhoFinal.Models
{
    /// <summary>
    /// Representa um serviço adicionado ao carrinho do cliente.
    /// </summary>
    public class CarrinhoServico
    {
        /// <summary>
        /// Identificador do item de serviço no carrinho.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador do carrinho.
        /// </summary>
        public int CarrinhoId { get; set; }

        /// <summary>
        /// Identificador do serviço.
        /// </summary>
        public int ServicoId { get; set; }

        /// <summary>
        /// Preço do serviço no momento em que foi adicionado ao carrinho.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoNoMomento { get; set; }

        /// <summary>
        /// Data em que o serviço foi adicionado ao carrinho.
        /// </summary>
        public DateTime DataAdicao { get; set; } = DateTime.Now;

        /// <summary>
        /// Carrinho associado.
        /// </summary>
        public Carrinho Carrinho { get; set; } = null!;

        /// <summary>
        /// Serviço associado.
        /// </summary>
        public Servico Servico { get; set; } = null!;
    }
}
