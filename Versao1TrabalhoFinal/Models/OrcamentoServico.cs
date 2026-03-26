using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Services
{
    /// <summary>
    /// Serviço com regras auxiliares de orçamentos.
    /// </summary>
    public class OrcamentoService
    {
        /// <summary>
        /// Recalcula o valor total estimado com base nos itens.
        /// </summary>
        /// <param name="orcamento">Orçamento a recalcular.</param>
        public void RecalcularValorEstimado(Orcamento orcamento)
        {
            if (orcamento.Descricao == null || !orcamento.Descricao.Any())
            {
                orcamento.ValorEstimado = 0;
                return;
            }

            //orcamento.ValorEstimado = orcamento.Itens.Sum(i => i.Quantidade * i.Preco);
        }

        /// <summary>
        /// Verifica se o estado é válido.
        /// </summary>
        /// <param name="estado">Estado a validar.</param>
        /// <returns>Verdadeiro se for um estado suportado.</returns>
        public bool EstadoValido(string? estado)
        {
            return estado == EstadoOrcamento.Pendente
                || estado == EstadoOrcamento.EmAnalise
                || estado == EstadoOrcamento.Aprovado
                || estado == EstadoOrcamento.Rejeitado;
        }
    }
}
