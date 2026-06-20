using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Api.DTOs.Common;
using Versao1TrabalhoFinal.Api.DTOs.VeiculosStand;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Api.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão de veículos disponíveis no stand.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class VeiculosStandController : ControllerBase
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Inicializa uma nova instância do controlador de veículos do stand.
        /// </summary>
        /// <param name="context">Contexto da base de dados da aplicação.</param>
        public VeiculosStandController(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém uma lista paginada de veículos do stand.
        /// </summary>
        /// <param name="pagination">Parâmetros de paginação.</param>
        /// <returns>Lista paginada de veículos do stand.</returns>
        [HttpGet]
        public async Task<ActionResult<PagedResponseDto<VeiculoStandReadDto>>> GetAll([FromQuery] PaginationQueryDto pagination)
        {
            var query = _context.VeiculosStand
                .AsNoTracking()
                .Include(vs => vs.Veiculo)
                .OrderBy(vs => vs.Id)
                .Select(vs => new VeiculoStandReadDto
                {
                    Id = vs.Id,
                    VeiculoId = vs.VeiculoId,
                    Marca = vs.Veiculo != null ? vs.Veiculo.Marca : null,
                    Modelo = vs.Veiculo != null ? vs.Veiculo.Modelo : null,
                    Matricula = vs.Veiculo != null ? vs.Veiculo.Matricula : null,
                    Preco = vs.Preco,
                    Estado = vs.Estado
                });

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            var response = new PagedResponseDto<VeiculoStandReadDto>
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
        /// Obtém um veículo do stand pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do registo do veículo no stand.</param>
        /// <returns>Os dados do veículo do stand, caso exista.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<VeiculoStandReadDto>> GetById(int id)
        {
            var veiculoStand = await _context.VeiculosStand
                .AsNoTracking()
                .Include(vs => vs.Veiculo)
                .Where(vs => vs.Id == id)
                .Select(vs => new VeiculoStandReadDto
                {
                    Id = vs.Id,
                    VeiculoId = vs.VeiculoId,
                    Marca = vs.Veiculo != null ? vs.Veiculo.Marca : null,
                    Modelo = vs.Veiculo != null ? vs.Veiculo.Modelo : null,
                    Matricula = vs.Veiculo != null ? vs.Veiculo.Matricula : null,
                    Preco = vs.Preco,
                    Estado = vs.Estado
                })
                .FirstOrDefaultAsync();

            if (veiculoStand == null)
            {
                return NotFound(new { message = "Veículo do stand não encontrado." });
            }

            return Ok(veiculoStand);
        }

        /// <summary>
        /// Cria um novo registo de veículo no stand.
        /// </summary>
        /// <param name="dto">Dados necessários para criar o registo do veículo no stand.</param>
        /// <returns>O registo criado.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<ActionResult<VeiculoStandReadDto>> Create(VeiculoStandCreateDto dto)
        {
            var veiculoExiste = await _context.Veiculos.AnyAsync(v => v.Id == dto.VeiculoId);
            if (!veiculoExiste)
            {
                return BadRequest(new { message = "O veículo indicado não existe." });
            }

            var veiculoStand = new VeiculoStand
            {
                VeiculoId = dto.VeiculoId,
                Preco = dto.Preco,
                Estado = dto.Estado
            };

            _context.VeiculosStand.Add(veiculoStand);
            await _context.SaveChangesAsync();

            var result = await _context.VeiculosStand
                .AsNoTracking()
                .Include(vs => vs.Veiculo)
                .Where(vs => vs.Id == veiculoStand.Id)
                .Select(vs => new VeiculoStandReadDto
                {
                    Id = vs.Id,
                    VeiculoId = vs.VeiculoId,
                    Marca = vs.Veiculo != null ? vs.Veiculo.Marca : null,
                    Modelo = vs.Veiculo != null ? vs.Veiculo.Modelo : null,
                    Matricula = vs.Veiculo != null ? vs.Veiculo.Matricula : null,
                    Preco = vs.Preco,
                    Estado = vs.Estado
                })
                .FirstAsync();

            return CreatedAtAction(nameof(GetById), new { id = veiculoStand.Id }, result);
        }

        /// <summary>
        /// Atualiza um registo de veículo do stand existente.
        /// </summary>
        /// <param name="id">Identificador do registo a atualizar.</param>
        /// <param name="dto">Dados atualizados do veículo do stand.</param>
        /// <returns>Mensagem de sucesso caso a atualização seja concluída.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> Update(int id, VeiculoStandUpdateDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest(new { message = "O identificador não corresponde ao veículo do stand." });
            }

            var veiculoStand = await _context.VeiculosStand.FindAsync(id);
            if (veiculoStand == null)
            {
                return NotFound(new { message = "Veículo do stand não encontrado." });
            }

            var veiculoExiste = await _context.Veiculos.AnyAsync(v => v.Id == dto.VeiculoId);
            if (!veiculoExiste)
            {
                return BadRequest(new { message = "O veículo indicado não existe." });
            }

            veiculoStand.VeiculoId = dto.VeiculoId;
            veiculoStand.Preco = dto.Preco;
            veiculoStand.Estado = dto.Estado;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Veículo do stand atualizado com sucesso." });
        }

        /// <summary>
        /// Remove um veículo do stand pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do registo a remover.</param>
        /// <returns>Mensagem de sucesso caso a remoção seja concluída.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var veiculoStand = await _context.VeiculosStand.FindAsync(id);
            if (veiculoStand == null)
            {
                return NotFound(new { message = "Veículo do stand não encontrado." });
            }

            _context.VeiculosStand.Remove(veiculoStand);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Veículo do stand removido com sucesso." });
        }
    }
}