using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;
using ClienteEntity = Versao1TrabalhoFinal.Models.Cliente;

namespace Versao1TrabalhoFinal.Pages.GestaoOrcamentos
{
    /// <summary>
    /// Página responsável pela criação de um pedido de orçamento por parte do cliente autenticado.
    /// </summary>
    [Authorize(Roles = "Cliente")]
    public class CreateModel : PageModel
    {
        /// <summary>
        /// Contexto da base de dados da aplicação.
        /// </summary>
        private readonly StandDbContext _context;

        /// <summary>
        /// Serviço de gestão de utilizadores do ASP.NET Core Identity.
        /// </summary>
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Inicializa uma nova instância da página de criação de pedidos de orçamento.
        /// </summary>
        /// <param name="context">Contexto da base de dados da aplicação.</param>
        /// <param name="userManager">Serviço de gestão de utilizadores do ASP.NET Core Identity.</param>
        public CreateModel(StandDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Dados do formulário de criação do pedido.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; } = new();

        /// <summary>
        /// Lista de veículos pertencentes ao cliente autenticado.
        /// </summary>
        public SelectList VeiculosOptions { get; set; } = default!;

        /// <summary>
        /// Carrega a página e os veículos do cliente autenticado.
        /// </summary>
        /// <returns>Resultado da execução da página.</returns>
        public async Task<IActionResult> OnGetAsync()
        {
            // Obtém o cliente associado ao utilizador autenticado.
            var cliente = await ObterClienteAutenticadoAsync();

            // Se não existir cliente associado, redireciona para a página inicial.
            if (cliente == null)
            {
                TempData["ErrorMessage"] = "Não foi encontrado um cliente associado à conta autenticada.";
                return RedirectToPage("/Index");
            }

            // Carrega os veículos do cliente para o dropdown.
            await CarregarVeiculosAsync(cliente.Id);

            return Page();
        }

        /// <summary>
        /// Processa a submissão do pedido de orçamento.
        /// </summary>
        /// <returns>Redireciona para a listagem após criação com sucesso.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            // Obtém o cliente autenticado.
            var cliente = await ObterClienteAutenticadoAsync();

            // Se não existir cliente associado, redireciona para a página inicial.
            if (cliente == null)
            {
                TempData["ErrorMessage"] = "Não foi encontrado um cliente associado à conta autenticada.";
                return RedirectToPage("/Index");
            }

            // Verifica se o veículo selecionado pertence efetivamente ao cliente autenticado.
            var veiculoValido = await _context.Veiculos
                .AnyAsync(v => v.Id == Input.VeiculoId && v.ClienteId == cliente.Id);

            // Se o veículo não for válido, adiciona erro ao ModelState.
            if (!veiculoValido)
            {
                ModelState.AddModelError("Input.VeiculoId", "Tem de selecionar um veículo válido.");
            }

            // Se o formulário não for válido, volta a carregar o dropdown e mostra a página novamente.
            if (!ModelState.IsValid)
            {
                await CarregarVeiculosAsync(cliente.Id);
                return Page();
            }

            // Cria a nova entidade de orçamento.
            var orcamento = new Orcamento
            {
                ClienteId = cliente.Id,
                VeiculoId = Input.VeiculoId,
                Descricao = Input.Descricao,
                ValorEstimado = null,
                GeradoPorIA = false,
                DataCriacao = DateTime.Now,
                Estado = "Pendente"
            };

            // Guarda o novo orçamento na base de dados.
            _context.Orcamentos.Add(orcamento);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Pedido de orçamento submetido com sucesso.";
            return RedirectToPage("./Index");
        }

        /// <summary>
        /// Obtém o cliente associado ao utilizador autenticado.
        /// </summary>
        /// <returns>Cliente autenticado ou null.</returns>
        private async Task<ClienteEntity?> ObterClienteAutenticadoAsync()
        {
            // Obtém o utilizador autenticado do Identity.
            var user = await _userManager.GetUserAsync(User);

            // Se não existir utilizador autenticado, devolve null.
            if (user == null)
            {
                return null;
            }

            // Procura o cliente associado ao IdentityUser autenticado.
            return await _context.Clientes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.IdentityUserId == user.Id);
        }

        /// <summary>
        /// Carrega os veículos do cliente para o dropdown do formulário.
        /// </summary>
        /// <param name="clienteId">Identificador do cliente.</param>
        private async Task CarregarVeiculosAsync(int clienteId)
        {
            // Obtém os veículos do cliente ordenados pela matrícula.
            var veiculos = await _context.Veiculos
                .AsNoTracking()
                .Where(v => v.ClienteId == clienteId)
                .OrderBy(v => v.Matricula)
                .Select(v => new
                {
                    v.Id,
                    Nome = v.Matricula + " - " + v.Marca + " " + v.Modelo
                })
                .ToListAsync();

            // Preenche a SelectList usada no dropdown da Razor Page.
            VeiculosOptions = new SelectList(veiculos, "Id", "Nome");
        }

        /// <summary>
        /// Modelo de dados utilizado no formulário de criação.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            /// Identificador do veículo selecionado.
            /// </summary>
            [Required(ErrorMessage = "Tem de selecionar um veículo.")]
            [Display(Name = "Veículo")]
            public int VeiculoId { get; set; }

            /// <summary>
            /// Descrição do problema identificado pelo cliente.
            /// </summary>
            [Required(ErrorMessage = "A descrição é obrigatória.")]
            [StringLength(500, ErrorMessage = "A descrição não pode ultrapassar os 500 caracteres.")]
            [Display(Name = "Descrição do problema")]
            public string Descricao { get; set; } = string.Empty;
        }
    }
}