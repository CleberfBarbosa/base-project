using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Handlers
{
    public static class JwtTokenHandler
    {
        public static string GenerateToken(string jwtKey, int expireMinutes, List<Claim> claims)
        {
            var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
            var symmetricKey = new SymmetricSecurityKey(keyBytes);

            var signingCredentials = new SigningCredentials(
                symmetricKey,
                SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.Now.ToUniversalTime().AddMinutes(expireMinutes);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: expiration,
                signingCredentials: signingCredentials);

            var tokenJwt = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenJwt;
        }
    }
}
