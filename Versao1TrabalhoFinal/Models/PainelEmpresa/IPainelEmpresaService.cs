using Versao1TrabalhoFinal.Models.PainelEmpresa;

namespace Versao1TrabalhoFinal.Services
{
    /// <summary>
    /// Contrato do serviço responsável por carregar e atualizar dados do painel da empresa.
    /// </summary>
    public interface IPainelEmpresaService
    {
        /// <summary>
        /// Carrega todos os dados necessários ao painel principal.
        /// </summary>
        /// <returns>ViewModel completo do painel.</returns>
        Task<PainelEmpresaViewModel> ObterPainelAsync();

        /// <summary>
        /// Atualiza o estado e o valor estimado de um orçamento.
        /// </summary>
        /// <param name="input">Dados da atualização.</param>
        /// <returns>Resultado da operação.</returns>
        Task<PainelOperacaoResultado> AtualizarOrcamentoAsync(AtualizarOrcamentoInputModel input);

        /// <summary>
        /// Atualiza o estado de uma ordem de reparação.
        /// </summary>
        /// <param name="input">Dados da atualização.</param>
        /// <returns>Resultado da operação.</returns>
        Task<PainelOperacaoResultado> AtualizarOrdemAsync(AtualizarOrdemInputModel input);
    }
}