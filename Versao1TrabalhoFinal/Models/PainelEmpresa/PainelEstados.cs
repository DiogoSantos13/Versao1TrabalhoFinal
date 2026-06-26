namespace Versao1TrabalhoFinal.Models.PainelEmpresa
{
    /// <summary>
    /// Centraliza estados usados no painel da empresa.
    /// </summary>
    public static class PainelEstados
    {
        /// <summary>
        /// Estados disponíveis para ordens de reparação.
        /// </summary>
        public static readonly string[] EstadosOrdem =
        {
            "Pendente",
            "Em Diagnóstico",
            "Em Reparação",
            "Aguarda Peças",
            "Concluído",
            "Entregue"
        };

        /// <summary>
        /// Estados disponíveis para orçamentos.
        /// </summary>
        public static readonly string[] EstadosOrcamento =
        {
            "Pendente",
            "Em Análise",
            "Aprovado",
            "Rejeitado"
        };
    }
}