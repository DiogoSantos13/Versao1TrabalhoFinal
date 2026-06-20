using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Api.DTOs.Common;
using Versao1TrabalhoFinal.Api.DTOs.PedidosContacto;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Api.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão de pedidos de contacto.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosContactoController : ControllerBase
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Inicializa uma nova instância do controlador de pedidos de contacto.
        /// </summary>
        /// <param name="context">Contexto da base de dados da aplicação.</param>
        public PedidosContactoController(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém uma lista paginada de pedidos de contacto.
        /// </summary>
        /// <param name="pagination">Parâmetros de paginação.</param>
        /// <returns>Lista paginada de pedidos de contacto.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<ActionResult<PagedResponseDto<PedidoContactoReadDto>>> GetAll([FromQuery] PaginationQueryDto pagination)
        {
            var query = _context.PedidosContacto
                .AsNoTracking()
                .OrderByDescending(p => p.DataCriacao)
                .Select(p => new PedidoContactoReadDto
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Email = p.Email,
                    Telefone = p.Telefone,
                    Mensagem = p.Mensagem,
                    DataCriacao = p.DataCriacao
                });

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            var response = new PagedResponseDto<PedidoContactoReadDto>
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
        /// Obtém um pedido de contacto pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do pedido de contacto.</param>
        /// <returns>Os dados do pedido, caso exista.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<ActionResult<PedidoContactoReadDto>> GetById(int id)
        {
            var pedido = await _context.PedidosContacto
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new PedidoContactoReadDto
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Email = p.Email,
                    Telefone = p.Telefone,
                    Mensagem = p.Mensagem,
                    DataCriacao = p.DataCriacao
                })
                .FirstOrDefaultAsync();

            if (pedido == null)
            {
                return NotFound(new { message = "Pedido de contacto não encontrado." });
            }

            return Ok(pedido);
        }

        /// <summary>
        /// Cria um novo pedido de contacto.
        /// </summary>
        /// <param name="dto">Dados necessários para criar o pedido.</param>
        /// <returns>O pedido criado.</returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<PedidoContactoReadDto>> Create(PedidoContactoCreateDto dto)
        {
            var pedido = new PedidoContacto
            {
                Nome = dto.Nome,
                Email = dto.Email,
                Telefone = dto.Telefone,
                Mensagem = dto.Mensagem,
                DataCriacao = DateTime.UtcNow
            };

            _context.PedidosContacto.Add(pedido);
            await _context.SaveChangesAsync();

            var result = new PedidoContactoReadDto
            {
                Id = pedido.Id,
                Nome = pedido.Nome,
                Email = pedido.Email,
                Telefone = pedido.Telefone,
                Mensagem = pedido.Mensagem,
                DataCriacao = pedido.DataCriacao
            };

            return CreatedAtAction(nameof(GetById), new { id = pedido.Id }, result);
        }

        /// <summary>
        /// Atualiza o estado ou dados de um pedido de contacto existente.
        /// </summary>
        /// <param name="id">Identificador do pedido a atualizar.</param>
        /// <param name="dto">Dados atualizados do pedido.</param>
        /// <returns>Mensagem de sucesso caso a atualização seja concluída.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> Update(int id, PedidoContactoUpdateDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest(new { message = "O identificador não corresponde ao pedido de contacto." });
            }

            var pedido = await _context.PedidosContacto.FindAsync(id);
            if (pedido == null)
            {
                return NotFound(new { message = "Pedido de contacto não encontrado." });
            }

            pedido.Nome = dto.Nome;
            pedido.Email = dto.Email;
            pedido.Telefone = dto.Telefone;
            pedido.Mensagem = dto.Mensagem;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Pedido de contacto atualizado com sucesso." });
        }

        /// <summary>
        /// Remove um pedido de contacto pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do pedido a remover.</param>
        /// <returns>Mensagem de sucesso caso a remoção seja concluída.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var pedido = await _context.PedidosContacto.FindAsync(id);
            if (pedido == null)
            {
                return NotFound(new { message = "Pedido de contacto não encontrado." });
            }

            _context.PedidosContacto.Remove(pedido);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Pedido de contacto removido com sucesso." });
        }
    }
}