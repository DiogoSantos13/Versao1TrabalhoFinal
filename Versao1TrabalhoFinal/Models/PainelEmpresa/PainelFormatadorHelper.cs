using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Models.PainelEmpresa
{
    /// <summary>
    /// Métodos auxiliares para formatar informação do painel.
    /// </summary>
    public static class PainelFormatadorHelper
    {
        /// <summary>
        /// Monta uma descrição amigável de um veículo.
        /// </summary>
        /// <param name="veiculo">Veículo a formatar.</param>
        /// <returns>Texto pronto a mostrar.</returns>
        public static string FormatarVeiculo(Veiculo? veiculo)
        {
            if (veiculo == null)
            {
                return "Sem veículo";
            }

            return $"{veiculo.Marca} {veiculo.Modelo} - {veiculo.Matricula}";
        }

        /// <summary>
        /// Devolve um texto por omissão quando o valor está vazio.
        /// </summary>
        /// <param name="texto">Texto de entrada.</param>
        /// <param name="fallback">Texto alternativo.</param>
        /// <returns>Texto final a mostrar.</returns>
        public static string TextoOuPadrao(string? texto, string fallback = "—")
        {
            return string.IsNullOrWhiteSpace(texto) ? fallback : texto;
        }
    }
}