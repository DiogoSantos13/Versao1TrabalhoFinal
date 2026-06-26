namespace Versao1TrabalhoFinal.Models.PainelEmpresa
{
    /// <summary>
    /// Representa o resultado de uma operação executada no painel.
    /// </summary>
    public class PainelOperacaoResultado
    {
        /// <summary>
        /// Indica se a operação foi concluída com sucesso.
        /// </summary>
        public bool Sucesso { get; set; }

        /// <summary>
        /// Mensagem a apresentar ao utilizador.
        /// </summary>
        public string Mensagem { get; set; } = string.Empty;

        /// <summary>
        /// Código opcional de detalhe técnico ou funcional.
        /// </summary>
        public string? Codigo { get; set; }

        /// <summary>
        /// Cria um resultado de sucesso.
        /// </summary>
        /// <param name="mensagem">Mensagem de sucesso.</param>
        /// <returns>Resultado preenchido.</returns>
        public static PainelOperacaoResultado Ok(string mensagem)
        {
            return new PainelOperacaoResultado
            {
                Sucesso = true,
                Mensagem = mensagem
            };
        }

        /// <summary>
        /// Cria um resultado de erro.
        /// </summary>
        /// <param name="mensagem">Mensagem de erro.</param>
        /// <param name="codigo">Código opcional.</param>
        /// <returns>Resultado preenchido.</returns>
        public static PainelOperacaoResultado Falha(string mensagem, string? codigo = null)
        {
            return new PainelOperacaoResultado
            {
                Sucesso = false,
                Mensagem = mensagem,
                Codigo = codigo
            };
        }
    }
}