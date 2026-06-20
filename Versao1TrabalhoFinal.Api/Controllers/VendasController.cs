using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Api.DTOs.Common;
using Versao1TrabalhoFinal.Api.DTOs.Vendas;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Api.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão de vendas.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VendasController : ControllerBase
    {
        private readonly StandDbContext _context;

        /// <summary>
        /// Inicializa uma nova instância do controlador de vendas.
        /// </summary>
        /// <param name="context">Contexto da base de dados da aplicação.</param>
        public VendasController(StandDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém uma lista paginada de vendas.
        /// </summary>
        /// <param name="pagination">Parâmetros de paginação.</param>
        /// <returns>Lista paginada de vendas.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<ActionResult<PagedResponseDto<VendaReadDto>>> GetAll([FromQuery] PaginationQueryDto pagination)
        {
            var query = _context.Vendas
                .AsNoTracking()
                .Include(v => v.Cliente)
                .Include(v => v.Itens)
                    .ThenInclude(i => i.Produto)
                .Include(v => v.Itens)
                    .ThenInclude(i => i.Servico)
                .OrderBy(v => v.Id)
                .Select(v => new VendaReadDto
                {
                    Id = v.Id,
                    ClienteId = v.ClienteId,
                    ClienteNome = v.Cliente != null ? v.Cliente.Nome : null,
                    Total = v.Total,
                    Observacoes = v.Observacoes,
                    DataVenda = v.Data,
                    Itens = v.Itens.Select(i => new VendaItemReadDto
                    {
                        Id = i.Id,
                        VendaId = i.VendaId,
                        ProdutoId = i.ProdutoId,
                        ProdutoNome = i.Produto != null ? i.Produto.Nome : null,
                        ServicoId = i.ServicoId,
                        ServicoNome = i.Servico != null ? i.Servico.Nome : null,
                        Quantidade = i.Quantidade,
                        PrecoUnitario = i.PrecoUnitario
                    }).ToList()
                });

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            var response = new PagedResponseDto<VendaReadDto>
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
        /// Obtém uma venda pelo identificador.
        /// </summary>
        /// <param name="id">Identificador da venda.</param>
        /// <returns>Os dados da venda, caso exista.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<ActionResult<VendaReadDto>> GetById(int id)
        {
            var venda = await _context.Vendas
                .AsNoTracking()
                .Include(v => v.Cliente)
                .Include(v => v.Itens)
                    .ThenInclude(i => i.Produto)
                .Include(v => v.Itens)
                    .ThenInclude(i => i.Servico)
                .Where(v => v.Id == id)
                .Select(v => new VendaReadDto
                {
                    Id = v.Id,
                    ClienteId = v.ClienteId,
                    ClienteNome = v.Cliente != null ? v.Cliente.Nome : null,
                    Total = v.Total,
                    Observacoes = v.Observacoes,
                    DataVenda = v.Data,
                    Itens = v.Itens.Select(i => new VendaItemReadDto
                    {
                        Id = i.Id,
                        VendaId = i.VendaId,
                        ProdutoId = i.ProdutoId,
                        ProdutoNome = i.Produto != null ? i.Produto.Nome : null,
                        ServicoId = i.ServicoId,
                        ServicoNome = i.Servico != null ? i.Servico.Nome : null,
                        Quantidade = i.Quantidade,
                        PrecoUnitario = i.PrecoUnitario
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (venda == null)
            {
                return NotFound(new { message = "Venda não encontrada." });
            }

            return Ok(venda);
        }

        /// <summary>
        /// Cria uma nova venda.
        /// </summary>
        /// <param name="dto">Dados necessários para criar a venda.</param>
        /// <returns>A venda criada.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<ActionResult<VendaReadDto>> Create(VendaCreateDto dto)
        {
            var venda = new Venda
            {
                ClienteId = dto.ClienteId,
                Observacoes = dto.Observacoes,
                Data = DateTime.UtcNow,
                Itens = new List<VendaItem>()
            };

            foreach (var item in dto.Itens)
            {
                venda.Itens.Add(new VendaItem
                {
                    ProdutoId = item.ProdutoId,
                    ServicoId = item.ServicoId,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = item.PrecoUnitario
                });
            }

            venda.Total = venda.Itens.Sum(i => i.Quantidade * i.PrecoUnitario);

            _context.Vendas.Add(venda);
            await _context.SaveChangesAsync();

            var result = await _context.Vendas
                .AsNoTracking()
                .Include(v => v.Cliente)
                .Include(v => v.Itens)
                    .ThenInclude(i => i.Produto)
                .Include(v => v.Itens)
                    .ThenInclude(i => i.Servico)
                .Where(v => v.Id == venda.Id)
                .Select(v => new VendaReadDto
                {
                    Id = v.Id,
                    ClienteId = v.ClienteId,
                    ClienteNome = v.Cliente != null ? v.Cliente.Nome : null,
                    Total = v.Total,
                    Observacoes = v.Observacoes,
                    DataVenda = v.Data,
                    Itens = v.Itens.Select(i => new VendaItemReadDto
                    {
                        Id = i.Id,
                        VendaId = i.VendaId,
                        ProdutoId = i.ProdutoId,
                        ProdutoNome = i.Produto != null ? i.Produto.Nome : null,
                        ServicoId = i.ServicoId,
                        ServicoNome = i.Servico != null ? i.Servico.Nome : null,
                        Quantidade = i.Quantidade,
                        PrecoUnitario = i.PrecoUnitario
                    }).ToList()
                })
                .FirstAsync();

            return CreatedAtAction(nameof(GetById), new { id = venda.Id }, result);
        }

        /// <summary>
        /// Atualiza os dados principais de uma venda existente.
        /// </summary>
        /// <param name="id">Identificador da venda a atualizar.</param>
        /// <param name="dto">Dados atualizados da venda.</param>
        /// <returns>Mensagem de sucesso caso a atualização seja concluída.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> Update(int id, VendaUpdateDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest(new { message = "O identificador não corresponde à venda." });
            }

            var venda = await _context.Vendas.FindAsync(id);
            if (venda == null)
            {
                return NotFound(new { message = "Venda não encontrada." });
            }

            venda.ClienteId = dto.ClienteId;
            venda.Observacoes = dto.Observacoes;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Venda atualizada com sucesso." });
        }

        /// <summary>
        /// Remove uma venda pelo identificador.
        /// </summary>
        /// <param name="id">Identificador da venda a remover.</param>
        /// <returns>Mensagem de sucesso caso a remoção seja concluída.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var venda = await _context.Vendas
                .Include(v => v.Itens)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venda == null)
            {
                return NotFound(new { message = "Venda não encontrada." });
            }

            _context.Vendas.Remove(venda);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Venda removida com sucesso." });
        }
    }
}