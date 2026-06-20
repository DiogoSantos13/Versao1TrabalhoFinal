using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Models
{
    public class Servico 
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        public decimal PrecoBase { get; set; }
        public int? TempoEstimado { get; set; }
        public bool Ativo { get; set; }
        public string? ImagemUrl { get; set; }



    }
}
