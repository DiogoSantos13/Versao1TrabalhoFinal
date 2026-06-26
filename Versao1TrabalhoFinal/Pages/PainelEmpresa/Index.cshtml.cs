using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;
using Versao1TrabalhoFinal.Models.PainelEmpresa;

namespace Versao1TrabalhoFinal.Pages.PainelEmpresa
{
    /// <summary>
    /// Página interna do painel da empresa.
    /// Permite consultar e atualizar ordens de reparação, produtos da ordem,
    /// orçamentos, histórico de vendas e carrinhos de clientes.
    /// </summary>
    [Authorize(Policy = "StaffOnly")]
    public class IndexModel : PageModel
    {
        /// <summary>
        /// Contexto EF Core da aplicação.
        /// </summary>
        private readonly StandDbContext _context;

        /// <summary>
        /// Injeta o DbContext na página.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        public IndexModel(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// ViewModel principal do painel.
        /// </summary>
        public PainelEmpresaViewModel Painel { get; set; } = new();

        /// <summary>
        /// Lista de estados possíveis para orçamentos.
        /// </summary>
        public List<SelectListItem> EstadosOrcamento { get; set; } = new();

        /// <summary>
        /// Lista de estados possíveis para ordens de reparação.
        /// </summary>
        public List<SelectListItem> EstadosOrdem { get; set; } = new();

        /// <summary>
        /// Lista de colaboradores disponíveis para atribuir às ordens.
        /// </summary>
        public List<SelectListItem> ColaboradoresDisponiveis { get; set; } = new();

        /// <summary>
        /// Lista de produtos disponíveis para adicionar às ordens.
        /// </summary>
        public List<SelectListItem> ProdutosDisponiveis { get; set; } = new();

        /// <summary>
        /// Verifica explicitamente se o utilizador atual pertence ao staff.
        /// </summary>
        private bool UtilizadorEhStaff()
        {
            return User.Identity != null &&
                   User.Identity.IsAuthenticated &&
                   (
                       User.IsInRole("Admin") ||
                       User.IsInRole("Mecanico") ||
                       User.IsInRole("Colaborador") ||
                       User.IsInRole("Vendedor") ||
                       User.IsInRole("Rececionista") ||
                       User.IsInRole("Funcionario")
                   );
        }

        /// <summary>
        /// Carrega a página inicial do painel.
        /// </summary>
        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Account/Login");
            }

            if (!UtilizadorEhStaff())
            {
                return RedirectToPage("/AccessDenied");
            }

            await CarregarListasAsync();
            await CarregarPainelAsync();
            return Page();
        }

        /// <summary>
        /// Tenta converter texto para decimal aceitando formatos PT e Invariant.
        /// </summary>
        private bool TentarConverterDecimal(string? valorTexto, out decimal valor)
        {
            valor = 0m;

            if (string.IsNullOrWhiteSpace(valorTexto))
                return true;

            valorTexto = valorTexto.Trim();

            return decimal.TryParse(
                       valorTexto,
                       System.Globalization.NumberStyles.Any,
                       System.Globalization.CultureInfo.GetCultureInfo("pt-PT"),
                       out valor)
                   || decimal.TryParse(
                       valorTexto,
                       System.Globalization.NumberStyles.Any,
                       System.Globalization.CultureInfo.InvariantCulture,
                       out valor);
        }

        /// <summary>
        /// Atualiza um orçamento existente.
        /// </summary>
        public async Task<IActionResult> OnPostAtualizarOrcamentoAsync(AtualizarOrcamentoInputModel atualizacaoOrcamento)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Account/Login");
            }

            if (!UtilizadorEhStaff())
            {
                return RedirectToPage("/AccessDenied");
            }

            await CarregarListasAsync();

            if (!ModelState.IsValid)
            {
                await CarregarPainelAsync();
                return Page();
            }

            var orcamento = await _context.Orcamentos
                .FirstOrDefaultAsync(o => o.Id == atualizacaoOrcamento.OrcamentoId);

            if (orcamento == null)
            {
                ModelState.AddModelError(string.Empty, "O orçamento não foi encontrado.");
                await CarregarPainelAsync();
                return Page();
            }

            if (!TentarConverterDecimal(atualizacaoOrcamento.ValorEstimado, out var valorEstimado))
            {
                ModelState.AddModelError(string.Empty, "O valor estimado é inválido.");
                await CarregarPainelAsync();
                return Page();
            }

            orcamento.Estado = atualizacaoOrcamento.NovoEstado;

            if (!string.IsNullOrWhiteSpace(atualizacaoOrcamento.ValorEstimado))
            {
                orcamento.ValorEstimado = valorEstimado;
            }

            if (!string.IsNullOrWhiteSpace(atualizacaoOrcamento.DescricaoAdicional))
            {
                orcamento.Descricao = atualizacaoOrcamento.DescricaoAdicional.Trim();
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Orçamento atualizado com sucesso.";
            return RedirectToPage();
        }

        /// <summary>
        /// Atualiza uma ordem de reparação existente.
        /// </summary>
        public async Task<IActionResult> OnPostAtualizarOrdemAsync(AtualizarOrdemInputModel atualizacaoOrdem)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Account/Login");
            }

            if (!UtilizadorEhStaff())
            {
                return RedirectToPage("/AccessDenied");
            }

            await CarregarListasAsync();

            if (!ModelState.IsValid)
            {
                await CarregarPainelAsync();
                return Page();
            }

            var ordem = await _context.OrdensReparacao
                .FirstOrDefaultAsync(o => o.Id == atualizacaoOrdem.OrdemReparacaoId);

            if (ordem == null)
            {
                ModelState.AddModelError(string.Empty, "A ordem de reparação não foi encontrada.");
                await CarregarPainelAsync();
                return Page();
            }

            if (!TentarConverterDecimal(atualizacaoOrdem.MaoDeObra, out var maoDeObra))
            {
                ModelState.AddModelError(string.Empty, "O valor da mão de obra é inválido.");
                await CarregarPainelAsync();
                return Page();
            }

            if (!TentarConverterDecimal(atualizacaoOrdem.Pecas, out var pecas))
            {
                ModelState.AddModelError(string.Empty, "O valor das peças é inválido.");
                await CarregarPainelAsync();
                return Page();
            }

            var colaboradorExiste = await _context.Colaboradores
                .AnyAsync(c => c.Id == atualizacaoOrdem.ColaboradorId);

            if (!colaboradorExiste)
            {
                ModelState.AddModelError(string.Empty, "O colaborador selecionado não existe.");
                await CarregarPainelAsync();
                return Page();
            }

            ordem.ColaboradorId = atualizacaoOrdem.ColaboradorId;
            ordem.Estado = atualizacaoOrdem.NovoEstado;
            ordem.MaoDeObra = maoDeObra;
            ordem.Pecas = pecas;
            ordem.Total = maoDeObra + pecas;

            if (!string.IsNullOrWhiteSpace(atualizacaoOrdem.Observacoes))
            {
                ordem.DescricaoProblema = atualizacaoOrdem.Observacoes.Trim();
            }

            if (atualizacaoOrdem.NovoEstado == "Concluído" && ordem.DataSaida == null)
            {
                ordem.DataSaida = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Ordem de reparação atualizada com sucesso.";
            return RedirectToPage();
        }

        /// <summary>
        /// Adiciona um produto a uma ordem de reparação.
        /// </summary>
        public async Task<IActionResult> OnPostAdicionarProdutoOrdemAsync(AdicionarProdutoOrdemInputModel input)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Account/Login");
            }

            if (!UtilizadorEhStaff())
            {
                return RedirectToPage("/AccessDenied");
            }

            await CarregarListasAsync();

            if (!ModelState.IsValid)
            {
                await CarregarPainelAsync();
                return Page();
            }

            var ordem = await _context.OrdensReparacao
                .FirstOrDefaultAsync(o => o.Id == input.OrdemId);

            if (ordem == null)
            {
                ModelState.AddModelError(string.Empty, "A ordem não foi encontrada.");
                await CarregarPainelAsync();
                return Page();
            }

            var produto = await _context.Produtos
                .FirstOrDefaultAsync(p => p.Id == input.ProdutoId);

            if (produto == null)
            {
                ModelState.AddModelError(string.Empty, "O produto não foi encontrado.");
                await CarregarPainelAsync();
                return Page();
            }

            if (input.Quantidade <= 0)
            {
                ModelState.AddModelError(string.Empty, "A quantidade tem de ser maior que zero.");
                await CarregarPainelAsync();
                return Page();
            }

            if (produto.Stock < input.Quantidade)
            {
                ModelState.AddModelError(string.Empty, "Stock insuficiente para o produto selecionado.");
                await CarregarPainelAsync();
                return Page();
            }

            var ordemProduto = new Versao1TrabalhoFinal.Models.OrdemProduto
            {
                OrdemId = input.OrdemId,
                ProdutoId = input.ProdutoId,
                Quantidade = input.Quantidade,
                Preco = produto.Preco
            };

            _context.OrdemProdutos.Add(ordemProduto);

            produto.Stock -= input.Quantidade;

            ordem.Pecas += produto.Preco * input.Quantidade;
            ordem.Total = ordem.MaoDeObra + ordem.Pecas;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Produto adicionado à ordem com sucesso.";
            return RedirectToPage();
        }

        /// <summary>
        /// Elimina um produto associado a uma ordem de reparação.
        /// Ao eliminar, devolve a quantidade ao stock e atualiza o total da ordem.
        /// </summary>
        public async Task<IActionResult> OnPostEliminarProdutoOrdemAsync(EliminarProdutoOrdemInputModel input)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Account/Login");
            }

            if (!UtilizadorEhStaff())
            {
                return RedirectToPage("/AccessDenied");
            }

            await CarregarListasAsync();

            if (!ModelState.IsValid)
            {
                await CarregarPainelAsync();
                return Page();
            }

            var ordemProduto = await _context.OrdemProdutos
                .Include(op => op.Produto)
                .Include(op => op.OrdemReparacao)
                .FirstOrDefaultAsync(op => op.Id == input.OrdemProdutoId && op.OrdemId == input.OrdemId);

            if (ordemProduto == null)
            {
                ModelState.AddModelError(string.Empty, "O produto associado à ordem não foi encontrado.");
                await CarregarPainelAsync();
                return Page();
            }

            if (ordemProduto.Produto == null)
            {
                ModelState.AddModelError(string.Empty, "O produto associado não foi encontrado.");
                await CarregarPainelAsync();
                return Page();
            }

            if (ordemProduto.OrdemReparacao == null)
            {
                ModelState.AddModelError(string.Empty, "A ordem associada não foi encontrada.");
                await CarregarPainelAsync();
                return Page();
            }

            var valorRemovido = ordemProduto.Preco * ordemProduto.Quantidade;

            ordemProduto.Produto.Stock += ordemProduto.Quantidade;

            ordemProduto.OrdemReparacao.Pecas -= valorRemovido;
            if (ordemProduto.OrdemReparacao.Pecas < 0)
            {
                ordemProduto.OrdemReparacao.Pecas = 0;
            }

            ordemProduto.OrdemReparacao.Total =
                ordemProduto.OrdemReparacao.MaoDeObra + ordemProduto.OrdemReparacao.Pecas;

            _context.OrdemProdutos.Remove(ordemProduto);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Produto removido da ordem com sucesso.";
            return RedirectToPage();
        }

        /// <summary>
        /// Carrega listas auxiliares para dropdowns e seleção.
        /// </summary>
        private async Task CarregarListasAsync()
        {
            EstadosOrcamento = new List<SelectListItem>
            {
                new SelectListItem { Value = "Pendente", Text = "Pendente" },
                new SelectListItem { Value = "Em Análise", Text = "Em Análise" },
                new SelectListItem { Value = "Aprovado", Text = "Aprovado" },
                new SelectListItem { Value = "Rejeitado", Text = "Rejeitado" }
            };

            EstadosOrdem = new List<SelectListItem>
            {
                new SelectListItem { Value = "Pendente", Text = "Pendente" },
                new SelectListItem { Value = "Em Diagnóstico", Text = "Em Diagnóstico" },
                new SelectListItem { Value = "Em Reparação", Text = "Em Reparação" },
                new SelectListItem { Value = "Aguarda Peças", Text = "Aguarda Peças" },
                new SelectListItem { Value = "Concluído", Text = "Concluído" },
                new SelectListItem { Value = "Entregue", Text = "Entregue" }
            };

            ColaboradoresDisponiveis = await _context.Colaboradores
                .OrderBy(c => c.Nome)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Nome + (string.IsNullOrWhiteSpace(c.Cargo) ? "" : " - " + c.Cargo)
                })
                .ToListAsync();

            ProdutosDisponiveis = await _context.Produtos
                .Where(p => p.Stock > 0)
                .OrderBy(p => p.Nome)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nome + " - " + p.Preco.ToString("0.00") + " € (Stock: " + p.Stock + ")"
                })
                .ToListAsync();
        }

        /// <summary>
        /// Carrega todos os dados visíveis no painel.
        /// </summary>
        private async Task CarregarPainelAsync()
        {
            var ordens = await _context.OrdensReparacao
                .Include(o => o.Cliente)
                .Include(o => o.Veiculo)
                .Include(o => o.Colaborador)
                .Include(o => o.OrdemProdutos)
                    .ThenInclude(op => op.Produto)
                .OrderByDescending(o => o.DataEntrada)
                .Take(20)
                .ToListAsync();

            var orcamentos = await _context.Orcamentos
                .Include(o => o.Cliente)
                .Include(o => o.Veiculo)
                .OrderByDescending(o => o.DataCriacao)
                .Take(20)
                .ToListAsync();

            var vendas = await _context.Vendas
                .Include(v => v.Cliente)
                .OrderByDescending(v => v.DataVenda)
                .Take(20)
                .ToListAsync();

            var carrinhos = await _context.Carrinhos
                .Include(c => c.Cliente)
                .Include(c => c.Produtos)
                    .ThenInclude(cp => cp.Produto)
                .Include(c => c.Servicos)
                    .ThenInclude(cs => cs.Servico)
                .Include(c => c.CarrinhoVeiculosStand)
                    .ThenInclude(cv => cv.VeiculoStand)
                .OrderByDescending(c => c.DataCriacao)
                .Take(20)
                .ToListAsync();

            Painel = new PainelEmpresaViewModel
            {
                TotalVendas = await _context.Vendas.SumAsync(v => (decimal?)v.Total) ?? 0,
                TotalVendasFinalizadas = await _context.Vendas.CountAsync(),
                TotalOrcamentosPendentes = await _context.Orcamentos.CountAsync(o => o.Estado == "Pendente"),
                TotalOrdensAbertas = await _context.OrdensReparacao.CountAsync(o => o.Estado != "Concluído" && o.Estado != "Entregue"),

                OrdensReparacao = ordens.Select(o => new PainelOrdemReparacaoViewModel
                {
                    Id = o.Id,
                    ClienteNome = o.Cliente != null ? o.Cliente.Nome : "Sem cliente",
                    VeiculoDescricao = o.Veiculo != null
                        ? $"{o.Veiculo.Marca} {o.Veiculo.Modelo} - {o.Veiculo.Matricula}"
                        : "Sem veículo",
                    ServicosPedidos = !string.IsNullOrWhiteSpace(o.DescricaoProblema)
                        ? o.DescricaoProblema
                        : "Sem descrição",
                    MecanicoResponsavel = o.Colaborador != null ? o.Colaborador.Nome : "Por atribuir",
                    Estado = string.IsNullOrWhiteSpace(o.Estado) ? "Pendente" : o.Estado,
                    DataEntrada = o.DataEntrada,
                    Total = o.Total,
                    ColaboradorId = o.ColaboradorId,
                    MaoDeObra = o.MaoDeObra,
                    Pecas = o.Pecas,
                    Produtos = o.OrdemProdutos.Select(op => new PainelOrdemProdutoViewModel
                    {
                        Id = op.Id,
                        ProdutoId = op.ProdutoId,
                        ProdutoNome = op.Produto != null ? op.Produto.Nome : "Produto desconhecido",
                        Quantidade = op.Quantidade,
                        Preco = op.Preco
                    }).ToList()
                }).ToList(),

                Orcamentos = orcamentos.Select(o => new PainelOrcamentoViewModel
                {
                    Id = o.Id,
                    ClienteNome = o.Cliente != null ? o.Cliente.Nome : "Sem cliente",
                    VeiculoDescricao = o.Veiculo != null
                        ? $"{o.Veiculo.Marca} {o.Veiculo.Modelo} - {o.Veiculo.Matricula}"
                        : "Sem veículo",
                    Descricao = o.Descricao,
                    Estado = string.IsNullOrWhiteSpace(o.Estado) ? "Pendente" : o.Estado,
                    ValorEstimado = o.ValorEstimado,
                    DataCriacao = o.DataCriacao
                }).ToList(),

                Vendas = vendas.Select(v => new PainelVendaViewModel
                {
                    Id = v.Id,
                    ClienteNome = v.Cliente != null ? v.Cliente.Nome : "Sem cliente",
                    DataVenda = v.DataVenda,
                    Total = v.Total,
                    Tipo = string.IsNullOrWhiteSpace(v.Tipo) ? "Venda" : v.Tipo,
                    ItensResumo = "Itens temporariamente indisponíveis"
                }).ToList(),

                Carrinhos = carrinhos.Select(c => new PainelCarrinhoViewModel
                {
                    Id = c.Id,
                    ClienteId = c.ClienteId,
                    ClienteNome = c.Cliente != null ? c.Cliente.Nome : "Sem cliente",
                    DataCriacao = c.DataCriacao,

                    Produtos = c.Produtos.Select(p => new PainelCarrinhoProdutoViewModel
                    {
                        NomeProduto = p.Produto != null ? p.Produto.Nome : "Produto desconhecido",
                        Quantidade = p.Quantidade,
                        PrecoNoMomento = p.PrecoNoMomento,
                        DataAdicao = p.DataAdicao
                    }).ToList(),

                    Servicos = c.Servicos.Select(s => new PainelCarrinhoServicoViewModel
                    {
                        NomeServico = s.Servico != null ? s.Servico.Nome : "Serviço desconhecido",
                        PrecoNoMomento = s.PrecoNoMomento,
                        DataAdicao = s.DataAdicao
                    }).ToList(),

                    Veiculos = c.CarrinhoVeiculosStand.Select(v => new PainelCarrinhoVeiculoViewModel
                    {
                        VeiculoDescricao = v.VeiculoStand != null
                            ? $"Veículo Stand #{v.VeiculoStandId}"
                            : "Veículo desconhecido",
                        PrecoNoMomento = v.PrecoNoMomento,
                        DataAdicao = v.DataAdicao
                    }).ToList()
                }).ToList()
            };
        }
    }
}