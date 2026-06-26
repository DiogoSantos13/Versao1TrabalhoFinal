namespace Versao1TrabalhoFinal.Models.PainelEmpresa
{
    /// <summary>
    /// Classe auxiliar para traduzir estados em estilos visuais do painel.
    /// </summary>
    public static class PainelEstadoHelper
    {
        /// <summary>
        /// Obtém a configuração visual associada a um estado.
        /// </summary>
        /// <param name="estado">Estado textual.</param>
        /// <returns>Objeto com classe CSS e dados do estado.</returns>
        public static PainelTabelaEstadoViewModel ObterEstadoVisual(string? estado)
        {
            var valor = string.IsNullOrWhiteSpace(estado) ? "Pendente" : estado;

            return valor switch
            {
                "Pendente" => new PainelTabelaEstadoViewModel
                {
                    Estado = valor,
                    ClasseCss = "status-warning",
                    Icone = "fa-regular fa-clock"
                },

                "Em Análise" => new PainelTabelaEstadoViewModel
                {
                    Estado = valor,
                    ClasseCss = "status-info",
                    Icone = "fa-solid fa-magnifying-glass"
                },

                "Em Diagnóstico" => new PainelTabelaEstadoViewModel
                {
                    Estado = valor,
                    ClasseCss = "status-info",
                    Icone = "fa-solid fa-stethoscope"
                },

                "Em Reparação" => new PainelTabelaEstadoViewModel
                {
                    Estado = valor,
                    ClasseCss = "status-info",
                    Icone = "fa-solid fa-screwdriver-wrench"
                },

                "Aguarda Peças" => new PainelTabelaEstadoViewModel
                {
                    Estado = valor,
                    ClasseCss = "status-warning",
                    Icone = "fa-solid fa-box-open"
                },

                "Aprovado" => new PainelTabelaEstadoViewModel
                {
                    Estado = valor,
                    ClasseCss = "status-success",
                    Icone = "fa-solid fa-check"
                },

                "Concluído" => new PainelTabelaEstadoViewModel
                {
                    Estado = valor,
                    ClasseCss = "status-success",
                    Icone = "fa-solid fa-circle-check"
                },

                "Entregue" => new PainelTabelaEstadoViewModel
                {
                    Estado = valor,
                    ClasseCss = "status-success",
                    Icone = "fa-solid fa-truck"
                },

                "Rejeitado" => new PainelTabelaEstadoViewModel
                {
                    Estado = valor,
                    ClasseCss = "status-danger",
                    Icone = "fa-solid fa-xmark"
                },

                _ => new PainelTabelaEstadoViewModel
                {
                    Estado = valor,
                    ClasseCss = "status-neutral",
                    Icone = "fa-regular fa-circle"
                }
            };
        }
    }
}