using System.ComponentModel.DataAnnotations.Schema;

namespace Versao1TrabalhoFinal.Models
{
    /// <summary>
    /// Histórico das intervenções realizadas em ordens de reparação.
    /// Cada registo representa uma atualização/intervenção efetuada.
    /// </summary>
    public class HistoricoReparacao
    {
        /// <summary>
        /// Identificador único do registo histórico.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// FK da ordem de reparação a que este histórico pertence.
        /// </summary>
        public int OrdemReparacaoId { get; set; }

        /// <summary>
        /// Navegação para a ordem de reparação.
        /// </summary>
        public OrdemReparacao? OrdemReparacao { get; set; }

        /// <summary>
        /// FK do veículo associado à intervenção.
        /// </summary>
        public int? VeiculoId { get; set; }

        /// <summary>
        /// Navegação para o veículo.
        /// </summary>
        public Veiculo? Veiculo { get; set; }

        /// <summary>
        /// FK opcional do colaborador que executou ou registou a intervenção.
        /// </summary>
        public int? ColaboradorId { get; set; }

        /// <summary>
        /// Navegação para o colaborador.
        /// </summary>
        public Colaborador? Colaborador { get; set; }

        /// <summary>
        /// Problema reportado ou resumo do serviço pedido.
        /// </summary>
        public string? Problema { get; set; }

        /// <summary>
        /// Solução aplicada ou observações da intervenção.
        /// </summary>
        public string? Solucao { get; set; }

        /// <summary>
        /// Estado da ordem no momento deste registo histórico.
        /// </summary>
        public string? Estado { get; set; }

        /// <summary>
        /// Custo total desta intervenção.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal Custo { get; set; }

        /// <summary>
        /// Mão de obra registada nesta intervenção.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal MaoDeObra { get; set; }

        /// <summary>
        /// Valor de peças registado nesta intervenção.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal Pecas { get; set; }

        /// <summary>
        /// Tempo estimado/consumido em horas.
        /// Podes manter a zero se ainda não estiveres a usar este campo.
        /// </summary>
        public int TempoHoras { get; set; }

        /// <summary>
        /// Data em que a intervenção foi registada.
        /// </summary>
        public DateTime DataReparacao { get; set; }
    }
}