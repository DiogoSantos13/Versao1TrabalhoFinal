using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Data
{
    /// <summary>
    /// Contexto principal da base de dados da aplicação.
    /// </summary>
    public class StandDbContext : IdentityDbContext<IdentityUser>
    {
        /// <summary>
        /// Inicializa uma nova instância do contexto.
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
        /// Tabela de veículos dos utilizadores.
        /// </summary>
        public DbSet<Veiculo> Veiculos { get; set; }

        /// <summary>
        /// Tabela de veículos do stand.
        /// </summary>
        public DbSet<VeiculoStand> VeiculosStand { get; set; }

        /// <summary>
        /// Tabela de produtos.
        /// </summary>
        public DbSet<Produto> Produtos { get; set; }

        /// <summary>
        /// Tabela de serviços.
        /// </summary>
        public DbSet<Servico> Servicos { get; set; }

        /// <summary>
        /// Tabela de pedidos de contacto.
        /// </summary>
        public DbSet<PedidoContacto> PedidosContacto { get; set; }

        /// <summary>
        /// Tabela de itens do carrinho.
        /// </summary>
        public DbSet<CarrinhoItem> CarrinhoItens { get; set; }

        /// <summary>
        /// Tabela de orçamentos.
        /// </summary>
        public DbSet<Orcamento> Orcamentos { get; set; }

        /// <summary>
        /// Tabela de itens de orçamento.
        /// </summary>
        public DbSet<OrcamentoItem> OrcamentoItens { get; set; }

        /// <summary>
        /// Tabela de vendas.
        /// </summary>
        public DbSet<Venda> Vendas { get; set; }

        /// <summary>
        /// Tabela de itens de venda.
        /// </summary>
        public DbSet<VendaItem> VendaItens { get; set; }

        /// <summary>
        /// Tabela de imagens associadas a entidades.
        /// </summary>
        public DbSet<ImagemEntidade> ImagensEntidade { get; set; }

        /// <summary>
        /// Configura o modelo de dados e as relações entre entidades.
        /// </summary>
        /// <param name="modelBuilder">Builder do modelo.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /// <summary>
            /// Configura a relação one-to-one entre VeiculoStand e Veiculo.
            /// </summary>
            modelBuilder.Entity<VeiculoStand>()
                .HasOne(vs => vs.Veiculo)
                .WithOne()
                .HasForeignKey<VeiculoStand>(vs => vs.VeiculoId)
                .OnDelete(DeleteBehavior.Restrict);

            /// <summary>
            /// Configura a relação entre Veiculo e Cliente.
            /// </summary>
            modelBuilder.Entity<Veiculo>()
                .HasOne(v => v.Cliente)
                .WithMany(c => c.Veiculos)
                .HasForeignKey(v => v.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            /// <summary>
            /// Configura a relação entre Produto e Fornecedor.
            /// </summary>
            modelBuilder.Entity<Produto>()
                .HasOne(p => p.Fornecedor)
                .WithMany()
                .HasForeignKey(p => p.FornecedorId)
                .OnDelete(DeleteBehavior.Restrict);

            /// <summary>
            /// Configura a relação entre Orcamento e Cliente.
            /// </summary>
            modelBuilder.Entity<Orcamento>()
                .HasOne(o => o.Cliente)
                .WithMany()
                .HasForeignKey(o => o.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            /// <summary>
            /// Configura a relação entre Orcamento e Veiculo.
            /// </summary>
            modelBuilder.Entity<Orcamento>()
                .HasOne(o => o.Veiculo)
                .WithMany()
                .HasForeignKey(o => o.VeiculoId)
                .OnDelete(DeleteBehavior.Restrict);

            /// <summary>
            /// Configura a relação entre OrcamentoItem e Orcamento.
            /// </summary>
            modelBuilder.Entity<OrcamentoItem>()
                .HasOne(oi => oi.Orcamento)
                .WithMany(o => o.Itens)
                .HasForeignKey(oi => oi.OrcamentoId)
                .OnDelete(DeleteBehavior.Cascade);

            /// <summary>
            /// Configura a relação entre OrcamentoItem e Produto.
            /// </summary>
            modelBuilder.Entity<OrcamentoItem>()
                .HasOne(oi => oi.Produto)
                .WithMany()
                .HasForeignKey(oi => oi.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            /// <summary>
            /// Configura a relação entre OrcamentoItem e Servico.
            /// </summary>
            modelBuilder.Entity<OrcamentoItem>()
                .HasOne(oi => oi.Servico)
                .WithMany()
                .HasForeignKey(oi => oi.ServicoId)
                .OnDelete(DeleteBehavior.Restrict);

            /// <summary>
            /// Configura a relação entre Venda e Cliente.
            /// </summary>
            modelBuilder.Entity<Venda>()
                .HasOne(v => v.Cliente)
                .WithMany()
                .HasForeignKey(v => v.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            /// <summary>
            /// Configura a relação entre VendaItem e Venda.
            /// </summary>
            modelBuilder.Entity<VendaItem>()
                .HasOne(vi => vi.Venda)
                .WithMany(v => v.Itens)
                .HasForeignKey(vi => vi.VendaId)
                .OnDelete(DeleteBehavior.Cascade);

            /// <summary>
            /// Configura a relação entre VendaItem e Produto.
            /// </summary>
            modelBuilder.Entity<VendaItem>()
                .HasOne(vi => vi.Produto)
                .WithMany()
                .HasForeignKey(vi => vi.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            /// <summary>
            /// Configura a relação entre VendaItem e Servico.
            /// </summary>
            modelBuilder.Entity<VendaItem>()
                .HasOne(vi => vi.Servico)
                .WithMany()
                .HasForeignKey(vi => vi.ServicoId)
                .OnDelete(DeleteBehavior.Restrict);

            /// <summary>
            /// Configura a relação entre CarrinhoItem e Veiculo.
            /// </summary>
            modelBuilder.Entity<CarrinhoItem>()
                .HasOne(ci => ci.Veiculo)
                .WithMany()
                .HasForeignKey(ci => ci.VeiculoId)
                .OnDelete(DeleteBehavior.Restrict);

            /// <summary>
            /// Configura a relação entre CarrinhoItem e Produto.
            /// </summary>
            modelBuilder.Entity<CarrinhoItem>()
                .HasOne(ci => ci.Produto)
                .WithMany()
                .HasForeignKey(ci => ci.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            /// <summary>
            /// Configura a relação entre CarrinhoItem e Servico.
            /// </summary>
            modelBuilder.Entity<CarrinhoItem>()
                .HasOne(ci => ci.Servico)
                .WithMany()
                .HasForeignKey(ci => ci.ServicoId)
                .OnDelete(DeleteBehavior.Restrict);

            /// <summary>
            /// Configura a precisão decimal do preço do produto.
            /// </summary>
            modelBuilder.Entity<Produto>()
                .Property(p => p.Preco)
                .HasColumnType("decimal(18,2)");

            /// <summary>
            /// Configura a precisão decimal do preço base do serviço.
            /// </summary>
            modelBuilder.Entity<Servico>()
                .Property(s => s.PrecoBase)
                .HasColumnType("decimal(18,2)");

            /// <summary>
            /// Configura a precisão decimal do preço do veículo do stand.
            /// </summary>
            modelBuilder.Entity<VeiculoStand>()
                .Property(vs => vs.Preco)
                .HasColumnType("decimal(18,2)");

            /// <summary>
            /// Configura a precisão decimal do total do orçamento.
            /// </summary>
            modelBuilder.Entity<Orcamento>()
                .Property(o => o.Total)
                .HasColumnType("decimal(18,2)");

            /// <summary>
            /// Configura a precisão decimal do preço unitário do item de orçamento.
            /// </summary>
            modelBuilder.Entity<OrcamentoItem>()
                .Property(oi => oi.PrecoUnitario)
                .HasColumnType("decimal(18,2)");

            /// <summary>
            /// Configura a precisão decimal do total da venda.
            /// </summary>
            modelBuilder.Entity<Venda>()
                .Property(v => v.Total)
                .HasColumnType("decimal(18,2)");

            /// <summary>
            /// Configura a precisão decimal do preço unitário do item de venda.
            /// </summary>
            modelBuilder.Entity<VendaItem>()
                .Property(vi => vi.PrecoUnitario)
                .HasColumnType("decimal(18,2)");
        }
    }
}