using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using SnackAdmin.Domain;

namespace SnackAdmin.Services
{
    public class JwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(string restaurantName)
        {
            var claims = new[] // this is how info is stored in the JWT token
            {
            new Claim(JwtRegisteredClaimNames.Sub, restaurantName), // subject (principal of the JWT)
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // JWT id to prevent replaying the token
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(5),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateTokenWithStatus(Order order)
        {
            var claims = new[] // this is how info is stored in the JWT token
            {
            new Claim(JwtRegisteredClaimNames.Sub, order.Id.ToString()), // subject (principal of the JWT)
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT id to prevent replaying the token
            new Claim("Status", order.Status.ToString()) // status info encoded in the token directly as well
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(5),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
