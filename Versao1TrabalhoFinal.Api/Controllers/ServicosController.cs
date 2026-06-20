using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Api.DTOs.Common;
using Versao1TrabalhoFinal.Api.DTOs.Servicos;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Api.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão de serviços.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ServicosController : ControllerBase
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Inicializa uma nova instância do controlador de serviços.
        /// </summary>
        /// <param name="context">Contexto da base de dados da aplicação.</param>
        public ServicosController(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém uma lista paginada de serviços.
        /// </summary>
        /// <param name="pagination">Parâmetros de paginação.</param>
        /// <returns>Lista paginada de serviços.</returns>
        [HttpGet]
        public async Task<ActionResult<PagedResponseDto<ServicoReadDto>>> GetAll([FromQuery] PaginationQueryDto pagination)
        {
            var query = _context.Servicos
                .AsNoTracking()
                .OrderBy(s => s.Id)
                .Select(s => new ServicoReadDto
                {
                    Id = s.Id,
                    Nome = s.Nome,
                    Descricao = s.Descricao,
                    PrecoBase = s.PrecoBase 
                });

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            var response = new PagedResponseDto<ServicoReadDto>
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
        /// Obtém um serviço pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do serviço.</param>
        /// <returns>Os dados do serviço, caso exista.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ServicoReadDto>> GetById(int id)
        {
            var servico = await _context.Servicos
                .AsNoTracking()
                .Where(s => s.Id == id)
                .Select(s => new ServicoReadDto
                {
                    Id = s.Id,
                    Nome = s.Nome,
                    Descricao = s.Descricao,
                    PrecoBase = s.PrecoBase 
                })
                .FirstOrDefaultAsync();

            if (servico == null)
            {
                return NotFound(new { message = "Serviço não encontrado." });
            }

            return Ok(servico);
        }

        /// <summary>
        /// Cria um novo serviço.
        /// </summary>
        /// <param name="dto">Dados necessários para criar o serviço.</param>
        /// <returns>O serviço criado.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<ActionResult<ServicoReadDto>> Create(ServicoCreateDto dto)
        {
            var servico = new Servico
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                PrecoBase = dto.PrecoBase
            };

            _context.Servicos.Add(servico);
            await _context.SaveChangesAsync();

            var result = new ServicoReadDto
            {
                Id = servico.Id,
                Nome = servico.Nome,
                Descricao = servico.Descricao,
                PrecoBase = servico.PrecoBase
            };

            return CreatedAtAction(nameof(GetById), new { id = servico.Id }, result);
        }

        /// <summary>
        /// Atualiza os dados de um serviço existente.
        /// </summary>
        /// <param name="id">Identificador do serviço a atualizar.</param>
        /// <param name="dto">Dados atualizados do serviço.</param>
        /// <returns>Mensagem de sucesso caso a atualização seja concluída.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> Update(int id, ServicoUpdateDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest(new { message = "O identificador não corresponde ao serviço." });
            }

            var servico = await _context.Servicos.FindAsync(id);
            if (servico == null)
            {
                return NotFound(new { message = "Serviço não encontrado." });
            }

            servico.Nome = dto.Nome;
            servico.Descricao = dto.Descricao;
            servico.PrecoBase = dto.PrecoBase;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Serviço atualizado com sucesso." });
        }

        /// <summary>
        /// Remove um serviço pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do serviço a remover.</param>
        /// <returns>Mensagem de sucesso caso a remoção seja concluída.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var servico = await _context.Servicos.FindAsync(id);
            if (servico == null)
            {
                return NotFound(new { message = "Serviço não encontrado." });
            }

            _context.Servicos.Remove(servico);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Serviço removido com sucesso." });
        }
    }
}