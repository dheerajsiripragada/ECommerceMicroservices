using ECommerce.UserService.Interfaces;
using ECommerce.UserService.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECommerce.UserService.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            var jwtSettings = _configuration
                .GetSection("JwtSettings");

            var key = jwtSettings["Key"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expiryMinutes =
                int.Parse(jwtSettings["ExpiryMinutes"]!);

            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()
                ),
                new Claim(
                    ClaimTypes.Name,
                    user.Name
                ),
                new Claim(
                    ClaimTypes.Email,
                    user.Email
                ),
                new Claim(
                    ClaimTypes.Role,
                    user.Role
                )
            };

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key!)
            );

            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}