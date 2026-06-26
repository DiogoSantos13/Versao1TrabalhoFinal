using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Models.PainelEmpresa
{
    /// <summary>
    /// Modelo usado para atualizar o mecânico responsável e o estado da ordem.
    /// </summary>
    public class AtualizarOrdemInputModel
    {
        // <summary>
        /// Identificador da ordem.
        /// </summary>
        [Required]
        public int OrdemReparacaoId { get; set; }

        /// <summary>
        /// Mecânico selecionado no formulário.
        /// Nesta fase é apenas visual e não é persistido.
        /// </summary>
        [Required(ErrorMessage = "Selecione um colaborador.")]
        public int ColaboradorId { get; set; }
        /// <summary>
        /// Novo estado da ordem.
        /// </summary>
        [Required(ErrorMessage = "Selecione um estado.")]
        public string NovoEstado { get; set; } = string.Empty;
        /// <summary>
        /// Valor de mão de obra introduzido no formulário.
        /// Não é guardado em coluna própria; serve para calcular o total.
        /// </summary>
        [Range(0, 999999)]
        public string MaoDeObra { get; set; }

        /// <summary>
        /// Valor das peças introduzido no formulário.
        /// Não é guardado em coluna própria; serve para calcular o total.
        /// </summary>
        [Range(0, 999999)]
        public string Pecas { get; set; }

        [StringLength(1000)]
        public string? Observacoes { get; set; }
    }
}