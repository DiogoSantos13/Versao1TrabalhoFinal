using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Versao1TrabalhoFinal.Api.Models;

namespace Versao1TrabalhoFinal.Api.Services
{
    /// <summary>
    /// Serviço responsável pela geração de tokens JWT.
    /// </summary>
    public class JwtService
    {
        private readonly JwtSettings _jwtSettings;

        /// <summary>
        /// Inicializa uma nova instância do serviço JWT.
        /// </summary>
        /// <param name="options">Definições JWT carregadas da configuração.</param>
        public JwtService(IOptions<JwtSettings> options)
        {
            _jwtSettings = options.Value;
        }

        /// <summary>
        /// Gera um token JWT para o utilizador autenticado.
        /// </summary>
        /// <param name="user">Utilizador autenticado.</param>
        /// <param name="roles">Roles do utilizador.</param>
        /// <returns>Token JWT serializado.</returns>
        public string GenerateToken(IdentityUser user, IEnumerable<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? user.Email ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}