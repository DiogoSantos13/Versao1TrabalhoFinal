using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Models.PainelEmpresa
{
    /// <summary>
    /// Modelo de input para eliminar um produto de uma ordem de reparação.
    /// </summary>
    public class EliminarProdutoOrdemInputModel
    {
        /// <summary>
        /// Identificador do registo OrdemProduto a eliminar.
        /// </summary>
        [Required]
        public int OrdemProdutoId { get; set; }

        /// <summary>
        /// Identificador da ordem de reparação.
        /// </summary>
        [Required]
        public int OrdemId { get; set; }
    }
}