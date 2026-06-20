using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Versao1TrabalhoFinal.Api.DTOs.Carrinho;
using Versao1TrabalhoFinal.Data;
using Versao1TrabalhoFinal.Models;

namespace Versao1TrabalhoFinal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CarrinhoController : ControllerBase
    {
        private readonly StandDbContext _context;

        public CarrinhoController(StandDbContext context)
        {
            _context = context;
        }

        private string? GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CarrinhoItemReadDto>>> GetMyCart()
        {
            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new { message = "Utilizador não autenticado." });
            }

            var itens = await _context.CarrinhoItens
                .AsNoTracking()
                .Include(i => i.Produto)
                .Include(i => i.Servico)
                .Where(i => i.UserId == userId)
                .Select(i => new CarrinhoItemReadDto
                {
                    Id = i.Id,
                    ProdutoId = i.ProdutoId,
                    ProdutoNome = i.Produto != null ? i.Produto.Nome : null,
                    ServicoId = i.ServicoId,
                    ServicoNome = i.Servico != null ? i.Servico.Nome : null,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario
                })
                .ToListAsync();

            return Ok(itens);
        }

        [HttpPost("itens")]
        public async Task<ActionResult<IEnumerable<CarrinhoItemReadDto>>> AddItem(CarrinhoItemCreateDto dto)
        {
            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new { message = "Utilizador não autenticado." });
            }

            var itemExistente = await _context.CarrinhoItens.FirstOrDefaultAsync(i =>
                i.UserId == userId &&
                i.ProdutoId == dto.ProdutoId &&
                i.ServicoId == dto.ServicoId);

            if (itemExistente != null)
            {
                itemExistente.Quantidade += dto.Quantidade;
                itemExistente.PrecoUnitario = dto.PrecoUnitario;
            }
            else
            {
                _context.CarrinhoItens.Add(new CarrinhoItem
                {
                    UserId = userId,
                    ProdutoId = dto.ProdutoId,
                    ServicoId = dto.ServicoId,
                    Quantidade = dto.Quantidade,
                    PrecoUnitario = dto.PrecoUnitario
                });
            }

            await _context.SaveChangesAsync();
            return await GetMyCart();
        }

        [HttpPut("itens/{itemId}")]
        public async Task<ActionResult<IEnumerable<CarrinhoItemReadDto>>> UpdateItem(int itemId, CarrinhoItemUpdateDto dto)
        {
            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new { message = "Utilizador não autenticado." });
            }

            var item = await _context.CarrinhoItens.FirstOrDefaultAsync(i =>
                i.Id == itemId && i.UserId == userId);

            if (item == null)
            {
                return NotFound(new { message = "Item do carrinho não encontrado." });
            }

            item.Quantidade = dto.Quantidade;
            item.PrecoUnitario = dto.PrecoUnitario;

            await _context.SaveChangesAsync();
            return await GetMyCart();
        }

        [HttpDelete("itens/{itemId}")]
        public async Task<ActionResult<IEnumerable<CarrinhoItemReadDto>>> RemoveItem(int itemId)
        {
            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new { message = "Utilizador não autenticado." });
            }

            var item = await _context.CarrinhoItens.FirstOrDefaultAsync(i =>
                i.Id == itemId && i.UserId == userId);

            if (item == null)
            {
                return NotFound(new { message = "Item do carrinho não encontrado." });
            }

            _context.CarrinhoItens.Remove(item);
            await _context.SaveChangesAsync();

            return await GetMyCart();
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new { message = "Utilizador não autenticado." });
            }

            var itens = await _context.CarrinhoItens
                .Where(i => i.UserId == userId)
                .ToListAsync();

            if (!itens.Any())
            {
                return Ok(new { message = "O carrinho já se encontra vazio." });
            }

            _context.CarrinhoItens.RemoveRange(itens);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Carrinho esvaziado com sucesso." });
        }
    }
}