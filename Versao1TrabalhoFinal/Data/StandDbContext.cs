using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Data
{
    /// <summary>
    /// Contexto principal da base de dados da aplicação, baseado em IdentityDbContext.
    /// </summary>
    public class StandDbContext : IdentityDbContext
    {
        /// <summary>
        /// Construtor do contexto, recebe as opções configuradas pelo DI.
        /// </summary>
        /// <param name="options">Opções de configuração do contexto.</param>
        public StandDbContext(DbContextOptions<StandDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Tabela de clientes.
        /// </summary>
        public DbSet<Cliente> Clientes { get; set; } = null!;

        /// <summary>
        /// Tabela de veículos dos clientes.
        /// </summary>
        public DbSet<Veiculo> Veiculos { get; set; } = null!;

        /// <summary>
        /// Tabela de veículos disponíveis no stand.
        /// </summary>
        public DbSet<VeiculoStand> VeiculosStand { get; set; } = null!;

        /// <summary>
        /// Tabela de orçamentos.
        /// </summary>
        public DbSet<Orcamento> Orcamentos { get; set; } = null!;

        /// <summary>
        /// Tabela de histórico de chat associado a orçamentos.
        /// </summary>
        public DbSet<HistoricoChatOrcamento> HistoricoChatOrcamentos { get; set; } = null!;

        /// <summary>
        /// Tabela de histórico de reparações.
        /// </summary>
        public DbSet<HistoricoReparacao> HistoricoReparacoes { get; set; } = null!;

        /// <summary>
        /// Tabela de produtos.
        /// </summary>
        public DbSet<Produto> Produtos { get; set; } = null!;

        /// <summary>
        /// Tabela de vendas.
        /// </summary>
        public DbSet<Venda> Vendas { get; set; } = null!;

        /// <summary>
        /// Tabela de serviços.
        /// </summary>
        public DbSet<Servico> Servicos { get; set; } = null!;

        /// <summary>
        /// Tabela de ordens de reparação.
        /// </summary>
        public DbSet<OrdemReparacao> OrdensReparacao { get; set; } = null!;

        /// <summary>
        /// Tabela de fornecedores.
        /// </summary>
        public DbSet<Fornecedor> Fornecedores { get; set; } = null!;

        /// <summary>
        /// Tabela de carrinhos de clientes.
        /// </summary>
        public DbSet<Carrinho> Carrinhos { get; set; } = null!;

        /// <summary>
        /// Tabela de itens de serviços no carrinho.
        /// </summary>
        public DbSet<CarrinhoServico> CarrinhoServicos { get; set; } = null!;

        /// <summary>
        /// Tabela de itens de produtos no carrinho (tabela de ligação).
        /// </summary>
        public DbSet<CarrinhoProdutos> CarrinhoProdutos { get; set; } = null!;

        /// <summary>
        /// Configura o modelo da base de dados, mapeando entidades, tabelas e relacionamentos.
        /// </summary>
        /// <param name="builder">Construtor do modelo (ModelBuilder).</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Mapear entidades para tabelas específicas com o schema "dbo"
            builder.Entity<Veiculo>().ToTable("Veiculos", "dbo");
            builder.Entity<VeiculoStand>().ToTable("VeiculosStand", "dbo");
            builder.Entity<Cliente>().ToTable("Clientes", "dbo");
            builder.Entity<Orcamento>().ToTable("Orcamentos", "dbo");
            builder.Entity<HistoricoChatOrcamento>().ToTable("HistoricoChatOrcamentos", "dbo");
            builder.Entity<HistoricoReparacao>().ToTable("HistoricoReparacoes", "dbo");
            builder.Entity<Produto>().ToTable("Produtos", "dbo");
            builder.Entity<Venda>().ToTable("Vendas", "dbo");
            builder.Entity<Servico>().ToTable("Servicos", "dbo");
            builder.Entity<OrdemReparacao>().ToTable("OrdensReparacao", "dbo");
            builder.Entity<Fornecedor>().ToTable("Fornecedores", "dbo");
            builder.Entity<Carrinho>().ToTable("Carrinhos", "dbo");
            builder.Entity<CarrinhoServico>().ToTable("CarrinhoServicos", "dbo");
            builder.Entity<CarrinhoProdutos>().ToTable("CarrinhoProdutos", "dbo");

            // Configuração de tipos numéricos (decimal) para garantir precisão adequada
            builder.Entity<VeiculoStand>()
                .Property(v => v.Preco)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Produto>()
                .Property(p => p.Preco)
                .HasColumnType("decimal(18,2)");

          
            builder.Entity<CarrinhoProdutos>()
                .Property(cp => cp.PrecoNoMomento)
                .HasColumnType("decimal(18,2)");

            // Relação VeiculoStand -> Veiculo (um veículo pode estar em vários registos do stand)
            builder.Entity<VeiculoStand>()
                .HasOne(vs => vs.Veiculo)
                .WithMany()
                .HasForeignKey(vs => vs.VeiculoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relação Produto -> Fornecedor (um fornecedor tem muitos produtos)
            builder.Entity<Produto>()
                .HasOne(p => p.Fornecedor)
                .WithMany(f => f.Produtos)
                .HasForeignKey(p => p.FornecedorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relação Carrinho -> Cliente (um para um)
            builder.Entity<Carrinho>()
                .HasOne(c => c.Cliente)
                .WithOne(c => c.Carrinho)
                .HasForeignKey<Carrinho>(c => c.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índice único para garantir que cada cliente tem apenas um carrinho
            builder.Entity<Carrinho>()
                .HasIndex(c => c.ClienteId)
                .IsUnique();


            // Relação CarrinhoServico -> Carrinho (um carrinho tem muitos serviços)
            builder.Entity<CarrinhoServico>()
                .HasOne(cs => cs.Carrinho)
                .WithMany(c => c.Servicos)
                .HasForeignKey(cs => cs.CarrinhoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relação CarrinhoServico -> Servico
            builder.Entity<CarrinhoServico>()
                .HasOne(cs => cs.Servico)
                .WithMany(s => s.CarrinhoServicos)
                .HasForeignKey(cs => cs.ServicoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índice único para evitar o mesmo serviço repetido no mesmo carrinho
            builder.Entity<CarrinhoServico>()
                .HasIndex(cs => new { cs.CarrinhoId, cs.ServicoId })
                .IsUnique();

            // Relação CarrinhoProdutos -> Carrinho (um carrinho tem muitos produtos)
            builder.Entity<CarrinhoProdutos>()
                .HasOne(cp => cp.Carrinho)
                .WithMany(c => c.Produtos)
                .HasForeignKey(cp => cp.CarrinhoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relação CarrinhoProdutos -> Produto
            builder.Entity<CarrinhoProdutos>()
                .HasOne(cp => cp.Produto)
                .WithMany(p => p.CarrinhoProdutos)
                .HasForeignKey(cp => cp.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índice único para evitar o mesmo produto repetido no mesmo carrinho
            builder.Entity<CarrinhoProdutos>()
                .HasIndex(cp => new { cp.CarrinhoId, cp.ProdutoId })
                .IsUnique();
        }
    }
}