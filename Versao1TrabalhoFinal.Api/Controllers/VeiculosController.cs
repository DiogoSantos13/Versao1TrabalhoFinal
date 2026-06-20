using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Api.DTOs.Common;
using Versao1TrabalhoFinal.Api.DTOs.Veiculos;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Api.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão de veículos.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class VeiculosController : ControllerBase
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Inicializa uma nova instância do controlador de veículos.
        /// </summary>
        /// <param name="context">Contexto da base de dados da aplicação.</param>
        public VeiculosController(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém uma lista paginada de veículos.
        /// </summary>
        /// <param name="pagination">Parâmetros de paginação.</param>
        /// <returns>Lista paginada de veículos.</returns>
        [HttpGet]
        public async Task<ActionResult<PagedResponseDto<VeiculoReadDto>>> GetAll([FromQuery] PaginationQueryDto pagination)
        {
            var query = _context.Veiculos
                .AsNoTracking()
                .Include(v => v.Cliente)
                .OrderBy(v => v.Id)
                .Select(v => new VeiculoReadDto
                {
                    Id = v.Id,
                    Marca = v.Marca,
                    Modelo = v.Modelo,
                    Matricula = v.Matricula,
                    Ano = v.Ano,
                    Combustivel = v.Combustivel,
                    Quilometros = v.quilometragem,
                    ClienteId = v.ClienteId,
                    ClienteNome = v.Cliente != null ? v.Cliente.Nome : null
                });

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            var response = new PagedResponseDto<VeiculoReadDto>
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
        /// Obtém um veículo pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do veículo.</param>
        /// <returns>Os dados do veículo, caso exista.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<VeiculoReadDto>> GetById(int id)
        {
            var veiculo = await _context.Veiculos
                .AsNoTracking()
                .Include(v => v.Cliente)
                .Where(v => v.Id == id)
                .Select(v => new VeiculoReadDto
                {
                    Id = v.Id,
                    Marca = v.Marca,
                    Modelo = v.Modelo,
                    Matricula = v.Matricula,
                    Ano = v.Ano,
                    Combustivel = v.Combustivel,
                    Quilometros = v.quilometragem,
                    ClienteId = v.ClienteId,
                    ClienteNome = v.Cliente != null ? v.Cliente.Nome : null
                })
                .FirstOrDefaultAsync();

            if (veiculo == null)
            {
                return NotFound(new { message = "Veículo não encontrado." });
            }

            return Ok(veiculo);
        }

        /// <summary>
        /// Cria um novo veículo.
        /// </summary>
        /// <param name="dto">Dados necessários para criar o veículo.</param>
        /// <returns>O veículo criado.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<ActionResult<VeiculoReadDto>> Create(VeiculoCreateDto dto)
        {
            var veiculo = new Veiculo
            {
                Marca = dto.Marca,
                Modelo = dto.Modelo,
                Matricula = dto.Matricula,
                Ano = dto.Ano,
                Combustivel = dto.Combustivel,
                quilometragem = dto.Quilometros,
                ClienteId = dto.ClienteId
            };

            _context.Veiculos.Add(veiculo);
            await _context.SaveChangesAsync();

            var result = await _context.Veiculos
                .AsNoTracking()
                .Include(v => v.Cliente)
                .Where(v => v.Id == veiculo.Id)
                .Select(v => new VeiculoReadDto
                {
                    Id = v.Id,
                    Marca = v.Marca,
                    Modelo = v.Modelo,
                    Matricula = v.Matricula,
                    Ano = v.Ano,
                    Combustivel = v.Combustivel,
                    Quilometros = v.quilometragem,
                    ClienteId = v.ClienteId,
                    ClienteNome = v.Cliente != null ? v.Cliente.Nome : null
                })
                .FirstAsync();

            return CreatedAtAction(nameof(GetById), new { id = veiculo.Id }, result);
        }

        /// <summary>
        /// Atualiza os dados de um veículo existente.
        /// </summary>
        /// <param name="id">Identificador do veículo a atualizar.</param>
        /// <param name="dto">Dados atualizados do veículo.</param>
        /// <returns>Mensagem de sucesso caso a atualização seja concluída.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> Update(int id, VeiculoUpdateDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest(new { message = "O identificador não corresponde ao veículo." });
            }

            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo == null)
            {
                return NotFound(new { message = "Veículo não encontrado." });
            }

            veiculo.Marca = dto.Marca;
            veiculo.Modelo = dto.Modelo;
            veiculo.Matricula = dto.Matricula;
            veiculo.Ano = dto.Ano;
            veiculo.Combustivel = dto.Combustivel;
            veiculo.quilometragem = dto.Quilometros;
            veiculo.ClienteId = dto.ClienteId;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Veículo atualizado com sucesso." });
        }

        /// <summary>
        /// Remove um veículo pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do veículo a remover.</param>
        /// <returns>Mensagem de sucesso caso a remoção seja concluída.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo == null)
            {
                return NotFound(new { message = "Veículo não encontrado." });
            }

            _context.Veiculos.Remove(veiculo);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Veículo removido com sucesso." });
        }
    }
}