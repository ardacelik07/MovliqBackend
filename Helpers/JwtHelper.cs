using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using RunningApplicationNew.Entity;

namespace RunningApplicationNew.Helpers
{
    public class JwtHelper : IJwtHelper
    {
        private readonly string _secretKey;
        private const string Issuer = "YourAppName"; // Token sağlayıcı (ör: uygulama adı)
        private const string Audience = "YourAppNameUsers"; // Token tüketici (ör: kullanıcılar)

        public JwtHelper(IConfiguration configuration)
        {
            // AppSettings'den SecretKey al
            _secretKey = configuration.GetValue<string>("Jwt:SecretKey");
        }

        public  string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("userId", user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name + " " + user.SurName),
            new Claim("Username", user.UserName ?? string.Empty)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), // Token geçerlilik süresi
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
        public  string ValidateTokenAndGetEmail(string token)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

                // Email bilgisini token'dan al
                var emailClaim = jsonToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);

                return emailClaim?.Value; // Email döndür
            }
            catch
            {
                return null;  // Token geçersizse null döndür
            }
        }
        public string GeneratePasswordResetToken(string email)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.Email, email)
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.Now.AddHours(1), // 1 saat geçerli
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string ValidatePasswordResetToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ReadToken(token) as JwtSecurityToken;
                var emailClaim = principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);

                if (emailClaim == null)
                    return null;

                return emailClaim.Value;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
