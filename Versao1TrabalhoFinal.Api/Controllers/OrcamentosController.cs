using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Api.DTOs.Common;
using Versao1TrabalhoFinal.Api.DTOs.Orcamentos;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Api.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão de orçamentos.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrcamentosController : ControllerBase
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Inicializa uma nova instância do controlador de orçamentos.
        /// </summary>
        /// <param name="context">Contexto da base de dados da aplicação.</param>
        public OrcamentosController(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém uma lista paginada de orçamentos.
        /// </summary>
        /// <param name="pagination">Parâmetros de paginação.</param>
        /// <returns>Lista paginada de orçamentos.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<ActionResult<PagedResponseDto<OrcamentoReadDto>>> GetAll([FromQuery] PaginationQueryDto pagination)
        {
            var query = _context.Orcamentos
    .AsNoTracking()
    .Include(o => o.Cliente)
    .Include(o => o.Itens)
        .ThenInclude(i => i.Produto)
    .OrderBy(o => o.Id)
    .Select(o => new OrcamentoReadDto
    {
        Id = o.Id,
        ClienteId = o.ClienteId,
        ClienteNome = o.Cliente != null ? o.Cliente.Nome : null,
        DataCriacao = o.DataCriacao,
        Total = o.Total
       
    });

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            var response = new PagedResponseDto<OrcamentoReadDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = pagination.Page,
                PageSize = pagination.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pagination.PageSize)
            };

            return Ok(response);
        }

        /// <summary>
        /// Obtém um orçamento pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do orçamento.</param>
        /// <returns>Os dados do orçamento, caso exista.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<ActionResult<OrcamentoReadDto>> GetById(int id)
        {
            var orcamento = await _context.Orcamentos
                .AsNoTracking()
                .Include(o => o.Cliente)
                .Where(o => o.Id == id)
                .Select(o => new OrcamentoReadDto
                {
                    Id = o.Id,
                    ClienteId = o.ClienteId,
                    ClienteNome = o.Cliente != null ? o.Cliente.Nome : null,
                    DataCriacao = o.DataCriacao,
                    ValorTotal = o.Total
                })
                .FirstOrDefaultAsync();

            if (orcamento == null)
            {
                return NotFound(new { message = "Orçamento não encontrado." });
            }

            return Ok(orcamento);
        }

        /// <summary>
        /// Cria um novo orçamento.
        /// </summary>
        /// <param name="dto">Dados necessários para criar o orçamento.</param>
        /// <returns>O orçamento criado.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<ActionResult<OrcamentoReadDto>> Create(OrcamentoCreateDto dto)
        {
            var orcamento = new Orcamento
            {
                ClienteId = dto.ClienteId,
                DataCriacao = DateTime.UtcNow,
                Total = dto.ValorTotal
            };

            _context.Orcamentos.Add(orcamento);
            await _context.SaveChangesAsync();

            var result = await _context.Orcamentos
                .AsNoTracking()
                .Include(o => o.Cliente)
                .Where(o => o.Id == orcamento.Id)
                .Select(o => new OrcamentoReadDto
                {
                    Id = o.Id,
                    ClienteId = o.ClienteId,
                    ClienteNome = o.Cliente != null ? o.Cliente.Nome : null,
                    DataCriacao = o.DataCriacao,
                    ValorTotal = o.Total
                   })
                .FirstAsync();

            return CreatedAtAction(nameof(GetById), new { id = orcamento.Id }, result);
        }

        /// <summary>
        /// Atualiza os dados principais de um orçamento existente.
        /// </summary>
        /// <param name="id">Identificador do orçamento a atualizar.</param>
        /// <param name="dto">Dados atualizados do orçamento.</param>
        /// <returns>Mensagem de sucesso caso a atualização seja concluída.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> Update(int id, OrcamentoUpdateDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest(new { message = "O identificador não corresponde ao orçamento." });
            }

            var orcamento = await _context.Orcamentos.FindAsync(id);
            if (orcamento == null)
            {
                return NotFound(new { message = "Orçamento não encontrado." });
            }

            orcamento.ClienteId = dto.ClienteId;
            orcamento.Total = dto.ValorTotal;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Orçamento atualizado com sucesso." });
        }

        /// <summary>
        /// Remove um orçamento pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do orçamento a remover.</param>
        /// <returns>Mensagem de sucesso caso a remoção seja concluída.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var orcamento = await _context.Orcamentos.FindAsync(id);
            if (orcamento == null)
            {
                return NotFound(new { message = "Orçamento não encontrado." });
            }

            _context.Orcamentos.Remove(orcamento);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Orçamento removido com sucesso." });
        }
    }
}