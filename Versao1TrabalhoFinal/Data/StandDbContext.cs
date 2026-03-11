using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Data
{
    /// <summary>
    /// Contexto principal da aplicação.
    /// Inclui as tabelas do Identity e as tabelas de negócio.
    /// </summary>
    public class StandDbContext : IdentityDbContext<IdentityUser>
    {
        /// <summary>
        /// Inicializa uma nova instância do contexto da aplicação.
        /// </summary>
        /// <param name="options">Opções de configuração do contexto.</param>
        public StandDbContext(DbContextOptions<StandDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Tabela de clientes.
        /// </summary>
        public DbSet<Cliente> Clientes { get; set; }

        /// <summary>
        /// Tabela de fornecedores.
        /// </summary>
        public DbSet<Fornecedor> Fornecedores { get; set; }

        /// <summary>
        /// Tabela de histórico de reparações.
        /// </summary>
        public DbSet<HistoricoReparacao> HistoricoReparacoes { get; set; }

        /// <summary>
        /// Tabela de itens de orçamento.
        /// </summary>
        public DbSet<OrcamentoItem> OrcamentoItens { get; set; }

        /// <summary>
        /// Tabela de orçamentos.
        /// </summary>
        public DbSet<Orcamento> Orcamentos { get; set; }

        /// <summary>
        /// Tabela de associação entre ordens e produtos.
        /// </summary>
        public DbSet<OrdemProduto> OrdemProdutos { get; set; }

        /// <summary>
        /// Tabela de ordens de reparação.
        /// </summary>
        public DbSet<OrdemReparacao> OrdensReparacao { get; set; }

        /// <summary>
        /// Tabela de produtos.
        /// </summary>
        public DbSet<Produto> Produtos { get; set; }

        /// <summary>
        /// Tabela de serviços.
        /// </summary>
        public DbSet<Servico> Servicos { get; set; }

        /// <summary>
        /// Tabela de veículos.
        /// </summary>
        public DbSet<Veiculo> Veiculos { get; set; }

        /// <summary>
        /// Tabela de veículos do stand.
        /// </summary>
        public DbSet<VeiculoStand> VeiculosStand { get; set; }

        /// <summary>
        /// Tabela de itens de venda.
        /// </summary>
        public DbSet<VendaItem> VendaItens { get; set; }

        /// <summary>
        /// Tabela de vendas.
        /// </summary>
        public DbSet<Venda> Vendas { get; set; }

        /// <summary>
        /// Configura as relações entre entidades.
        /// </summary>
        /// <param name="modelBuilder">Builder do modelo.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Veiculo>()
                .HasOne(v => v.Cliente)
                .WithMany(c => c.Veiculos)
                .HasForeignKey(v => v.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VeiculoStand>()
                .HasOne(vs => vs.Veiculo)
                .WithMany(v => v.VeiculosStand)
                .HasForeignKey(vs => vs.VeiculoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Produto>()
                .HasOne(p => p.Fornecedor)
                .WithMany(f => f.Produtos)
                .HasForeignKey(p => p.FornecedorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrdemReparacao>()
                .HasOne(o => o.Cliente)
                .WithMany(c => c.OrdensReparacao)
                .HasForeignKey(o => o.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrdemReparacao>()
                .HasOne(o => o.Veiculo)
                .WithMany(v => v.OrdensReparacao)
                .HasForeignKey(o => o.VeiculoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrdemProduto>()
                .HasOne(op => op.OrdemReparacao)
                .WithMany(o => o.OrdemProdutos)
                .HasForeignKey(op => op.OrdemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrdemProduto>()
                .HasOne(op => op.Produto)
                .WithMany(p => p.OrdemProdutos)
                .HasForeignKey(op => op.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Orcamento>()
                .HasOne(o => o.Cliente)
                .WithMany(c => c.Orcamentos)
                .HasForeignKey(o => o.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Orcamento>()
                .HasOne(o => o.Veiculo)
                .WithMany(v => v.Orcamentos)
                .HasForeignKey(o => o.VeiculoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrcamentoItem>()
                .HasOne(oi => oi.Orcamento)
                .WithMany(o => o.Itens)
                .HasForeignKey(oi => oi.OrcamentoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrcamentoItem>()
                .HasOne(oi => oi.Servico)
                .WithMany(s => s.OrcamentoItens)
                .HasForeignKey(oi => oi.ServicoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrcamentoItem>()
                .HasOne(oi => oi.Produto)
                .WithMany(p => p.OrcamentoItens)
                .HasForeignKey(oi => oi.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Venda>()
                .HasOne(v => v.Cliente)
                .WithMany(c => c.Vendas)
                .HasForeignKey(v => v.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VendaItem>()
                .HasOne(vi => vi.Venda)
                .WithMany(v => v.Itens)
                .HasForeignKey(vi => vi.VendaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<VendaItem>()
                .HasOne(vi => vi.Produto)
                .WithMany(p => p.VendaItens)
                .HasForeignKey(vi => vi.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HistoricoReparacao>()
                .HasOne(h => h.Veiculo)
                .WithMany(v => v.HistoricoReparacoes)
                .HasForeignKey(h => h.VeiculoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
