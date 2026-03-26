using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Data
{
    /// <summary>
    /// Contexto principal da base de dados da aplicação.
    /// </summary>
    public class StandDbContext : IdentityDbContext
    {
        /// <summary>
        /// Inicializa uma nova instância do contexto da base de dados.
        /// </summary>
        /// <param name="options">Opções de configuração do contexto.</param>
        public StandDbContext(DbContextOptions<StandDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Clientes registados na aplicação.
        /// </summary>
        public DbSet<Cliente> Clientes { get; set; }

        /// <summary>
        /// Veículos associados aos clientes.
        /// </summary>
        public DbSet<Veiculo> Veiculos { get; set; }

        /// <summary>
        /// Veículos disponíveis no stand.
        /// </summary>
        public DbSet<VeiculoStand> VeiculosStand { get; set; }

        /// <summary>
        /// Orçamentos registados na aplicação.
        /// </summary>
        public DbSet<Orcamento> Orcamentos { get; set; }

        /// <summary>
        /// Histórico de mensagens do chat de orçamentos.
        /// </summary>
        public DbSet<HistoricoChatOrcamento> HistoricoChatOrcamentos { get; set; }

        /// <summary>
        /// Histórico de reparações efetuadas.
        /// </summary>
        public DbSet<HistoricoReparacao> HistoricoReparacoes { get; set; }

        /// <summary>
        /// Produtos disponíveis na aplicação.
        /// </summary>
        public DbSet<Produto> Produtos { get; set; }

        /// <summary>
        /// Vendas registadas.
        /// </summary>
        public DbSet<Venda> Vendas { get; set; }

        /// <summary>
        /// Serviços disponíveis.
        /// </summary>
        public DbSet<Servico> Servicos { get; set; }

        /// <summary>
        /// Ordens de reparação registadas.
        /// </summary>
        public DbSet<OrdemReparacao> OrdensReparacao { get; set; }

        /// <summary>
        /// Fornecedores registados na aplicação.
        /// </summary>
        public DbSet<Fornecedor> Fornecedores { get; set; }

        /// <summary>
        /// Carrinhos dos clientes.
        /// </summary>
        public DbSet<Carrinho> Carrinhos { get; set; }

        /// <summary>
        /// Itens dos carrinhos.
        /// </summary>
        public DbSet<CarrinhoItem> CarrinhoItens { get; set; }

        /// <summary>
        /// Itens dos carrinhos serviços.
        /// </summary>
        public DbSet<CarrinhoServico> CarrinhoServicos { get; set; }


        /// <summary>
        /// Configura o modelo da base de dados e os relacionamentos entre entidades.
        /// </summary>
        /// <param name="builder">Construtor do modelo.</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

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
            builder.Entity<CarrinhoItem>().ToTable("CarrinhoItens", "dbo");

            builder.Entity<VeiculoStand>()
                .Property(v => v.Preco)
                .HasColumnType("decimal(18,2)");

            builder.Entity<CarrinhoItem>()
                .Property(ci => ci.PrecoNoMomento)
                .HasColumnType("decimal(18,2)");

            builder.Entity<VeiculoStand>()
                .HasOne(vs => vs.Veiculo)
                .WithMany()
                .HasForeignKey(vs => vs.VeiculoId)
                .OnDelete(DeleteBehavior.Restrict);

           

            builder.Entity<Carrinho>()
                .HasOne(c => c.Cliente)
                .WithOne(c => c.Carrinho)
                .HasForeignKey<Carrinho>(c => c.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Carrinho>()
                .HasIndex(c => c.ClienteId)
                .IsUnique();

            builder.Entity<CarrinhoItem>()
                .HasOne(ci => ci.Carrinho)
                .WithMany(c => c.Itens)
                .HasForeignKey(ci => ci.CarrinhoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CarrinhoItem>()
                .HasOne(ci => ci.VeiculoStand)
                .WithMany(vs => vs.CarrinhoItens)
                .HasForeignKey(ci => ci.VeiculoStandId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CarrinhoItem>()
                .HasIndex(ci => new { ci.CarrinhoId, ci.VeiculoStandId })
                .IsUnique();

            builder.Entity<CarrinhoServico>()
    .HasKey(cs => cs.Id);

            builder.Entity<CarrinhoServico>()
                .HasOne(cs => cs.Carrinho)
                .WithMany(c => c.Servicos)
                .HasForeignKey(cs => cs.CarrinhoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CarrinhoServico>()
                .HasOne(cs => cs.Servico)
                .WithMany(s => s.CarrinhoServicos)
                .HasForeignKey(cs => cs.ServicoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CarrinhoServico>()
                .HasIndex(cs => new { cs.CarrinhoId, cs.ServicoId })
                .IsUnique();

        }
    }
}
