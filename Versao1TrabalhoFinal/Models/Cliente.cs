using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Telefone { get; set; }
        public string? Email { get; set; }
        public string? NIF { get; set; }
        public string? Morada { get; set; }
        public DateTime DataRegisto { get; set; }
        public string? ImagemUrl { get; set; }

        public ICollection<Veiculo> Veiculos { get; set; } = new List<Veiculo>();
        public ICollection<OrdemReparacao> OrdensReparacao { get; set; } = new List<OrdemReparacao>();
        public ICollection<Orcamento> Orcamentos { get; set; } = new List<Orcamento>();
        public ICollection<Venda> Vendas { get; set; } = new List<Venda>();
    }
}
