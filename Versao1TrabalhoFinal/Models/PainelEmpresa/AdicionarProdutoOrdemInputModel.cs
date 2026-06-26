using System.ComponentModel.DataAnnotations;

namespace Versao1TrabalhoFinal.Models.PainelEmpresa
{
    /// <summary>
    /// Modelo usado para submeter a adição de um produto
    /// a uma ordem de reparação no painel da empresa.
    /// </summary>
    public class AdicionarProdutoOrdemInputModel
    {
        /// <summary>
        /// Identificador da ordem de reparação.
        /// </summary>
        [Required]
        public int OrdemId { get; set; }

        /// <summary>
        /// Identificador do produto a adicionar.
        /// </summary>
        [Required]
        public int ProdutoId { get; set; }

        /// <summary>
        /// Quantidade do produto a adicionar à ordem.
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade tem de ser pelo menos 1.")]
        public int Quantidade { get; set; }
    }
}