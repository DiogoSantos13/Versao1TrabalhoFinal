using System.ComponentModel.DataAnnotations.Schema;

namespace Versao1TrabalhoFinal.Models
{
    /// <summary>
    /// Representa uma ordem de reparação de oficina.
    /// Guarda o estado atual do serviço, valores atuais e colaborador atribuído.
    /// </summary>
    public class OrdemReparacao
    {
        /// <summary>
        /// Identificador único da ordem.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// FK opcional para o cliente.
        /// Nullable para compatibilidade com dados antigos ou cenários incompletos.
        /// </summary>
        public int? ClienteId { get; set; }

        /// <summary>
        /// Cliente associado à ordem.
        /// </summary>
        public Cliente? Cliente { get; set; }

        /// <summary>
        /// FK opcional para o veículo.
        /// </summary>
        public int? VeiculoId { get; set; }

        /// <summary>
        /// Veículo associado à ordem.
        /// </summary>
        public Veiculo? Veiculo { get; set; }

        /// <summary>
        /// Data de entrada da viatura / abertura da ordem.
        /// </summary>
        public DateTime DataEntrada { get; set; }

        /// <summary>
        /// Data de saída da viatura, quando aplicável.
        /// </summary>
        public DateTime? DataSaida { get; set; }

        /// <summary>
        /// Estado atual da ordem.
        /// Ex.: Pendente, Em Reparação, Concluído.
        /// </summary>
        public string? Estado { get; set; }

        /// <summary>
        /// Descrição do problema reportado pelo cliente.
        /// </summary>
        public string? DescricaoProblema { get; set; }

        /// <summary>
        /// Total atual da ordem.
        /// Normalmente corresponde a mão de obra + peças.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        /// <summary>
        /// FK opcional para o colaborador/mecânico atualmente atribuído.
        /// </summary>
        public int? ColaboradorId { get; set; }

        /// <summary>
        /// Navegação para o colaborador atualmente responsável.
        /// </summary>
        public Colaborador? Colaborador { get; set; }

        /// <summary>
        /// Valor atual de mão de obra associado à ordem.
        /// Este valor fica persistido para voltar a aparecer no painel.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal MaoDeObra { get; set; }

        /// <summary>
        /// Valor atual de peças associado à ordem.
        /// Este valor fica persistido para voltar a aparecer no painel.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal Pecas { get; set; }

        /// <summary>
        /// Produtos associados à ordem, caso estejas a usar essa funcionalidade.
        /// </summary>
        public ICollection<OrdemProduto> OrdemProdutos { get; set; } = new List<OrdemProduto>();

        /// <summary>
        /// Histórico das intervenções efetuadas nesta ordem.
        /// </summary>
        public ICollection<HistoricoReparacao> HistoricoReparacoes { get; set; } = new List<HistoricoReparacao>();
    }
}