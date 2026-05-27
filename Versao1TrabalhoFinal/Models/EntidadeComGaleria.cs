using System.ComponentModel.DataAnnotations.Schema;

namespace Versao1TrabalhoFinal.Models
{
    /// <summary>
    /// Classe base para entidades que suportam galeria de imagens.
    /// A lista de imagens é carregada manualmente e não é mapeada pelo EF Core.
    /// </summary>
    public abstract class EntidadeComGaleria
    {
        /// <summary>
        /// Lista de imagens associadas à entidade.
        /// Esta propriedade não é mapeada para a base de dados.
        /// </summary>
        [NotMapped]
        public List<ImagemEntidade> Imagens { get; set; } = new();
    }
}