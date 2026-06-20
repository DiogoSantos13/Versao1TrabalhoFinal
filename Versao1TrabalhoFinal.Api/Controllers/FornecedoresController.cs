using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Api.DTOs.Common;
using Versao1TrabalhoFinal.Api.DTOs.Fornecedores;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Api.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão de fornecedores.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class FornecedoresController : ControllerBase
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Inicializa uma nova instância do controlador de fornecedores.
        /// </summary>
        /// <param name="context">Contexto da base de dados da aplicação.</param>
        public FornecedoresController(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém uma lista paginada de fornecedores.
        /// </summary>
        /// <param name="pagination">Parâmetros de paginação.</param>
        /// <returns>Lista paginada de fornecedores.</returns>
        [HttpGet]
        public async Task<ActionResult<PagedResponseDto<FornecedorReadDto>>> GetAll([FromQuery] PaginationQueryDto pagination)
        {
            var query = _context.Fornecedores
                .AsNoTracking()
                .OrderBy(f => f.Id)
                .Select(f => new FornecedorReadDto
                {
                    Id = f.Id,
                    Nome = f.Nome,
                    Email = f.Email,
                    Telefone = f.Telefone,
                    Morada = f.Morada
                });

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            var response = new PagedResponseDto<FornecedorReadDto>
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
        /// Obtém um fornecedor pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do fornecedor.</param>
        /// <returns>Os dados do fornecedor, caso exista.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<FornecedorReadDto>> GetById(int id)
        {
            var fornecedor = await _context.Fornecedores
                .AsNoTracking()
                .Where(f => f.Id == id)
                .Select(f => new FornecedorReadDto
                {
                    Id = f.Id,
                    Nome = f.Nome,
                    Email = f.Email,
                    Telefone = f.Telefone,
                    Morada = f.Morada
                })
                .FirstOrDefaultAsync();

            if (fornecedor == null)
            {
                return NotFound(new { message = "Fornecedor não encontrado." });
            }

            return Ok(fornecedor);
        }

        /// <summary>
        /// Cria um novo fornecedor.
        /// </summary>
        /// <param name="dto">Dados necessários para criar o fornecedor.</param>
        /// <returns>O fornecedor criado.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<FornecedorReadDto>> Create(FornecedorCreateDto dto)
        {
            var fornecedor = new Fornecedor
            {
                Nome = dto.Nome,
                Email = dto.Email,
                Telefone = dto.Telefone,
                Morada = dto.Morada
            };

            _context.Fornecedores.Add(fornecedor);
            await _context.SaveChangesAsync();

            var result = new FornecedorReadDto
            {
                Id = fornecedor.Id,
                Nome = fornecedor.Nome,
                Email = fornecedor.Email,
                Telefone = fornecedor.Telefone,
                Morada = fornecedor.Morada
            };

            return CreatedAtAction(nameof(GetById), new { id = fornecedor.Id }, result);
        }

        /// <summary>
        /// Atualiza os dados de um fornecedor existente.
        /// </summary>
        /// <param name="id">Identificador do fornecedor a atualizar.</param>
        /// <param name="dto">Dados atualizados do fornecedor.</param>
        /// <returns>Mensagem de sucesso caso a atualização seja concluída.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, FornecedorUpdateDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest(new { message = "O identificador não corresponde ao fornecedor." });
            }

            var fornecedor = await _context.Fornecedores.FindAsync(id);
            if (fornecedor == null)
            {
                return NotFound(new { message = "Fornecedor não encontrado." });
            }

            fornecedor.Nome = dto.Nome;
            fornecedor.Email = dto.Email;
            fornecedor.Telefone = dto.Telefone;
            fornecedor.Morada = dto.Morada;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Fornecedor atualizado com sucesso." });
        }

        /// <summary>
        /// Remove um fornecedor pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do fornecedor a remover.</param>
        /// <returns>Mensagem de sucesso caso a remoção seja concluída.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var fornecedor = await _context.Fornecedores.FindAsync(id);
            if (fornecedor == null)
            {
                return NotFound(new { message = "Fornecedor não encontrado." });
            }

            _context.Fornecedores.Remove(fornecedor);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Fornecedor removido com sucesso." });
        }
    }
}