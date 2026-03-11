

namespace Versao1TrabalhoFinal.Models
{
    public class Fornecedor
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Telefone { get; set; }
        public string? Email { get; set; }
        public string? Morada { get; set; }

        public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
    }
}
