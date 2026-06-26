using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Models;
using ClienteEntity = Versao1TrabalhoFinal.Models.Cliente;

namespace Versao1TrabalhoFinal.Data
{
    /// <summary>
    /// Contexto principal da base de dados da aplicação.
    /// Herda de IdentityDbContext para suportar autenticação/autorização ASP.NET Identity.
    /// </summary>
    public class StandDbContext : IdentityDbContext
    {
        /// <summary>
        /// Inicializa uma nova instância do contexto.
        /// </summary>
        /// <param name="options">Opções de configuração injetadas pelo ASP.NET Core.</param>
        public StandDbContext(DbContextOptions<StandDbContext> options) : base(options)
        {
        }

        // =========================
        // DbSets principais
        // =========================

        public DbSet<ClienteEntity> Clientes { get; set; } = null!;
        public DbSet<Colaborador> Colaboradores { get; set; } = null!;
        public DbSet<Veiculo> Veiculos { get; set; } = null!;
        public DbSet<VeiculoStand> VeiculosStand { get; set; } = null!;
        public DbSet<Orcamento> Orcamentos { get; set; } = null!;
        public DbSet<HistoricoReparacao> HistoricoReparacoes { get; set; } = null!;
        public DbSet<Produto> Produtos { get; set; } = null!;
        public DbSet<Venda> Vendas { get; set; } = null!;
        public DbSet<VendaItem> VendaItens { get; set; } = null!;
        public DbSet<Servico> Servicos { get; set; } = null!;
        public DbSet<OrdemReparacao> OrdensReparacao { get; set; } = null!;
        public DbSet<OrdemProduto> OrdemProdutos { get; set; } = null!;
        public DbSet<Fornecedor> Fornecedores { get; set; } = null!;
        public DbSet<Carrinho> Carrinhos { get; set; } = null!;
        public DbSet<CarrinhoServico> CarrinhoServicos { get; set; } = null!;
        public DbSet<CarrinhoProdutos> CarrinhoProdutos { get; set; } = null!;
        public DbSet<CarrinhoVeiculoStand> CarrinhoVeiculosStand { get; set; } = null!;
        public DbSet<ImagemEntidade> ImagensEntidade => Set<ImagemEntidade>();

        /// <summary>
        /// Configura o modelo EF Core: nomes de tabelas, tipos, relações e índices.
        /// </summary>
        /// <param name="builder">ModelBuilder do EF Core.</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ==========================================================
            // Mapeamento explícito para tabelas no schema dbo
            // ==========================================================
            builder.Entity<ClienteEntity>().ToTable("Clientes", "dbo");
            builder.Entity<Colaborador>().ToTable("Colaboradores", "dbo");
            builder.Entity<Veiculo>().ToTable("Veiculos", "dbo");
            builder.Entity<VeiculoStand>().ToTable("VeiculosStand", "dbo");
            builder.Entity<Orcamento>().ToTable("Orcamentos", "dbo");
            builder.Entity<HistoricoReparacao>().ToTable("HistoricoReparacoes", "dbo");
            builder.Entity<Produto>().ToTable("Produtos", "dbo");
            builder.Entity<Venda>().ToTable("Vendas", "dbo");
            builder.Entity<VendaItem>().ToTable("VendaItens", "dbo");
            builder.Entity<Servico>().ToTable("Servicos", "dbo");
            builder.Entity<OrdemReparacao>().ToTable("OrdensReparacao", "dbo");
            builder.Entity<Fornecedor>().ToTable("Fornecedores", "dbo");
            builder.Entity<Carrinho>().ToTable("Carrinhos", "dbo");
            builder.Entity<CarrinhoServico>().ToTable("CarrinhoServicos", "dbo");
            builder.Entity<CarrinhoProdutos>().ToTable("CarrinhoProdutos", "dbo");
            builder.Entity<CarrinhoVeiculoStand>().ToTable("CarrinhoVeiculosStand", "dbo");
            builder.Entity<ImagemEntidade>().ToTable("ImagensEntidade", "dbo");

            // ==========================================================
            // Índices únicos de email / ligação ao utilizador Identity
            // ==========================================================
            builder.Entity<ClienteEntity>()
                .HasIndex(c => c.Email)
                .IsUnique();

            builder.Entity<Colaborador>()
                .HasIndex(c => c.Email)
                .IsUnique();

            builder.Entity<Colaborador>()
                .HasIndex(c => c.IdentityUserId)
                .IsUnique();

            // ==========================================================
            // Tipos decimais para evitar arredondamentos errados em SQL
            // ==========================================================
            builder.Entity<VeiculoStand>()
                .Property(v => v.Preco)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Produto>()
                .Property(p => p.Preco)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Venda>()
                .Property(v => v.Total)
                .HasColumnType("decimal(18,2)");

            builder.Entity<VendaItem>()
                .Property(vi => vi.Preco)
                .HasColumnType("decimal(18,2)");

            builder.Entity<CarrinhoProdutos>()
                .Property(cp => cp.PrecoNoMomento)
                .HasColumnType("decimal(18,2)");

            builder.Entity<CarrinhoServico>()
                .Property(cs => cs.PrecoNoMomento)
                .HasColumnType("decimal(18,2)");

            builder.Entity<CarrinhoVeiculoStand>()
                .Property(cvs => cvs.PrecoNoMomento)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Orcamento>()
                .Property(o => o.ValorEstimado)
                .HasColumnType("decimal(18,2)");

            builder.Entity<OrdemReparacao>()
                .Property(o => o.Total)
                .HasColumnType("decimal(18,2)");

            // ==========================================================
            // Campos persistidos da ordem de reparação para o painel
            // ==========================================================
            builder.Entity<OrdemReparacao>()
                .Property(o => o.MaoDeObra)
                .HasColumnType("decimal(18,2)");

            builder.Entity<OrdemReparacao>()
                .Property(o => o.Pecas)
                .HasColumnType("decimal(18,2)");
            // ==========================================================
            // Configuração da entidade OrdemProduto
            // ==========================================================
            builder.Entity<OrdemProduto>().ToTable("OrdemProdutos", "dbo");

            builder.Entity<OrdemProduto>()
                .HasKey(op => op.Id);

            builder.Entity<OrdemProduto>()
                .Property(op => op.Preco)
                .HasColumnType("decimal(18,2)");

            builder.Entity<OrdemProduto>()
                .HasOne(op => op.OrdemReparacao)
                .WithMany(o => o.OrdemProdutos)
                .HasForeignKey(op => op.OrdemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<OrdemProduto>()
                .HasOne(op => op.Produto)
                .WithMany(p => p.OrdemProdutos)
                .HasForeignKey(op => op.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<OrdemProduto>()
                .HasIndex(op => new { op.OrdemId, op.ProdutoId });
            // ==========================================================
            // Histórico de reparações com custos detalhados
            // ==========================================================
            builder.Entity<HistoricoReparacao>()
                .Property(h => h.Custo)
                .HasColumnType("decimal(18,2)");

            builder.Entity<HistoricoReparacao>()
                .Property(h => h.MaoDeObra)
                .HasColumnType("decimal(18,2)");

            builder.Entity<HistoricoReparacao>()
                .Property(h => h.Pecas)
                .HasColumnType("decimal(18,2)");

            // ==========================================================
            // Relação VeiculoStand -> Veiculo
            // ==========================================================
            builder.Entity<VeiculoStand>()
                .HasOne(vs => vs.Veiculo)
                .WithMany()
                .HasForeignKey(vs => vs.VeiculoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==========================================================
            // Relação Produto -> Fornecedor
            // ==========================================================
            builder.Entity<Produto>()
                .HasOne(p => p.Fornecedor)
                .WithMany(f => f.Produtos)
                .HasForeignKey(p => p.FornecedorId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==========================================================
            // Relação Venda -> Cliente
            // ==========================================================
            builder.Entity<Venda>()
                .HasOne(v => v.Cliente)
                .WithMany()
                .HasForeignKey(v => v.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==========================================================
            // Relação Venda -> VendaItem
            // ==========================================================
            builder.Entity<Venda>()
                .HasMany(v => v.Itens)
                .WithOne(i => i.Venda)
                .HasForeignKey(i => i.VendaId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==========================================================
            // Relação VendaItem -> Produto
            // ==========================================================
            builder.Entity<VendaItem>()
                .HasOne(vi => vi.Produto)
                .WithMany()
                .HasForeignKey(vi => vi.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==========================================================
            // Relação Carrinho -> Cliente (1 para 1)
            // ==========================================================
            builder.Entity<Carrinho>()
                .HasOne(c => c.Cliente)
                .WithOne(c => c.Carrinho)
                .HasForeignKey<Carrinho>(c => c.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Carrinho>()
                .HasIndex(c => c.ClienteId)
                .IsUnique();

            // ==========================================================
            // Relação CarrinhoServico -> Carrinho
            // ==========================================================
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

            // ==========================================================
            // Relação CarrinhoProdutos -> Carrinho
            // ==========================================================
            builder.Entity<CarrinhoProdutos>()
                .HasOne(cp => cp.Carrinho)
                .WithMany(c => c.Produtos)
                .HasForeignKey(cp => cp.CarrinhoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CarrinhoProdutos>()
                .HasOne(cp => cp.Produto)
                .WithMany(p => p.CarrinhoProdutos)
                .HasForeignKey(cp => cp.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CarrinhoProdutos>()
                .HasIndex(cp => new { cp.CarrinhoId, cp.ProdutoId })
                .IsUnique();

            // ==========================================================
            // Relação CarrinhoVeiculoStand -> Carrinho
            // ==========================================================
            builder.Entity<CarrinhoVeiculoStand>()
                .HasOne(cvs => cvs.Carrinho)
                .WithMany(c => c.CarrinhoVeiculosStand)
                .HasForeignKey(cvs => cvs.CarrinhoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CarrinhoVeiculoStand>()
                .HasOne(cvs => cvs.VeiculoStand)
                .WithMany(vs => vs.CarrinhoVeiculosStand)
                .HasForeignKey(cvs => cvs.VeiculoStandId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CarrinhoVeiculoStand>()
                .HasIndex(cvs => new { cvs.CarrinhoId, cvs.VeiculoStandId })
                .IsUnique();

            // ==========================================================
            // Relações da Ordem de Reparação
            // ==========================================================
            builder.Entity<OrdemReparacao>()
                .HasOne(o => o.Cliente)
                .WithMany()
                .HasForeignKey(o => o.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<OrdemReparacao>()
                .HasOne(o => o.Veiculo)
                .WithMany()
                .HasForeignKey(o => o.VeiculoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<OrdemReparacao>()
                .HasOne(o => o.Colaborador)
                .WithMany()
                .HasForeignKey(o => o.ColaboradorId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==========================================================
            // Relações do Orçamento
            // ==========================================================
            builder.Entity<Orcamento>()
                .HasOne(o => o.Cliente)
                .WithMany(c => c.Orcamentos)
                .HasForeignKey(o => o.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Orcamento>()
                .HasOne(o => o.Veiculo)
                .WithMany()
                .HasForeignKey(o => o.VeiculoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==========================================================
            // Relações do histórico de reparações
            // ==========================================================
            builder.Entity<HistoricoReparacao>()
                .HasOne(h => h.OrdemReparacao)
                .WithMany(o => o.HistoricoReparacoes)
                .HasForeignKey(h => h.OrdemReparacaoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<HistoricoReparacao>()
                .HasOne(h => h.Colaborador)
                .WithMany()
                .HasForeignKey(h => h.ColaboradorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<HistoricoReparacao>()
                .HasOne(h => h.Veiculo)
                .WithMany()
                .HasForeignKey(h => h.VeiculoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<HistoricoReparacao>()
                .HasIndex(h => h.OrdemReparacaoId);

            builder.Entity<HistoricoReparacao>()
                .HasIndex(h => h.ColaboradorId);

            builder.Entity<OrdemReparacao>()
                .HasIndex(o => o.ColaboradorId);

            // ==========================================================
            // Configuração da entidade de imagens
            // ==========================================================
            builder.Entity<ImagemEntidade>()
                .HasIndex(i => new { i.TipoEntidade, i.EntidadeId });

            builder.Entity<ImagemEntidade>()
                .Property(i => i.Url)
                .HasMaxLength(500)
                .IsRequired();

            builder.Entity<ImagemEntidade>()
                .Property(i => i.TipoEntidade)
                .HasMaxLength(50)
                .IsRequired();

            builder.Entity<ImagemEntidade>()
                .Property(i => i.Alt)
                .HasMaxLength(200);
        }
    }
}