using JwtAuthDemo.Startup;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JwtAuthDemo.Methods
{
    public class TokenOperations
    {
        private IConfiguration _configuration;

        public TokenOperations(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateToken(string email)
        {
            JwtSettings jwtSettings = _configuration.GetSection("JWT").Get<JwtSettings>();
            var key = Encoding.ASCII.GetBytes(jwtSettings.SecurityKey);

            var expiresTime = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings.ExpiresTime.ToString()));
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = jwtSettings.Audience,
                Issuer = jwtSettings.Issuer,
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Email", email),
                }),
                Expires = expiresTime,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

        public string DecodeToken(string token)
        {
            var tokenClaimEmail = string.Empty;
            var handler = new JwtSecurityTokenHandler();
            if (handler.ReadToken(token.Substring(7)) is JwtSecurityToken jwtToken)
            {
                tokenClaimEmail = jwtToken.Claims.First(claim => claim.Type == "Email").Value;
            }
            return tokenClaimEmail;
        }
    }
}
