namespace Versao1TrabalhoFinal.Models
{
    /// <summary>
    /// Representa um fornecedor de produtos.
    /// </summary>
    public class Fornecedor
    {
        /// <summary>
        /// Identificador único do fornecedor.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome do fornecedor.
        /// </summary>
        public string? Nome { get; set; }

        /// <summary>
        /// Número de telefone do fornecedor.
        /// </summary>
        public string? Telefone { get; set; }

        /// <summary>
        /// Endereço de email do fornecedor.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Endereço de morda do fornecedor.
        /// </summary>
        public string? Morada { get; set; }

        /// <summary>
        /// Coleção de produtos fornecidos por este fornecedor.
        /// </summary>
        public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
    }
}