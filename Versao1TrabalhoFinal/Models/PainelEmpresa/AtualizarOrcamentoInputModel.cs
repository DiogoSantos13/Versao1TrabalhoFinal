using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Models.PainelEmpresa
{
    /// <summary>
    /// Modelo usado para atualizar o estado de um orçamento a partir do painel.
    /// </summary>
    public class AtualizarOrcamentoInputModel
    {
        /// <summary>
        /// Identificador do orçamento.
        /// </summary>
        [Required]
        public int OrcamentoId { get; set; }

        /// <summary>
        /// Novo estado do orçamento.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string NovoEstado { get; set; } = string.Empty;

        /// <summary>
        /// Valor estimado a guardar no orçamento.
        /// </summary>
        public string? ValorEstimado { get; set; }

        /// <summary>
        /// Texto adicional introduzido por admin/mecânico.
        /// Será anexado à descrição já existente.
        /// </summary>
        [StringLength(1000)]
        public string? DescricaoAdicional { get; set; }
    }
}