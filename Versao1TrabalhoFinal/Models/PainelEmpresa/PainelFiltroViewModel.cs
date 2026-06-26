using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Models.PainelEmpresa
{
    /// <summary>
    /// Modelo de filtros do painel da empresa.
    /// </summary>
    public class PainelFiltroViewModel
    {
        /// <summary>
        /// Texto de pesquisa global.
        /// Pode procurar por cliente, matrícula ou descrição.
        /// </summary>
        [StringLength(150)]
        public string? Pesquisa { get; set; }

        /// <summary>
        /// Estado da ordem de reparação.
        /// </summary>
        [StringLength(50)]
        public string? EstadoOrdem { get; set; }

        /// <summary>
        /// Estado do orçamento.
        /// </summary>
        [StringLength(50)]
        public string? EstadoOrcamento { get; set; }

        /// <summary>
        /// Data inicial de filtro.
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime? DataInicio { get; set; }

        /// <summary>
        /// Data final de filtro.
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime? DataFim { get; set; }
    }
}