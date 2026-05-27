using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Models
{
    public class Servico : EntidadeComGaleria
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        public decimal PrecoBase { get; set; }
        public int TempoEstimado { get; set; }
        public bool Ativo { get; set; }
        public string? ImagemUrl { get; set; }

        public ICollection<OrcamentoItem> OrcamentoItens { get; set; } = new List<OrcamentoItem>();

        /// <summary>
        /// Registos de serviços associados a carrinhos.
        /// </summary>
        public ICollection<CarrinhoServico> CarrinhoServicos { get; set; } = new List<CarrinhoServico>();

    }
}
