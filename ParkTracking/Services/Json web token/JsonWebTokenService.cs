using Microsoft.IdentityModel.Tokens;
using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ParkTracking.Services.Json_web_token {
	public class JsonWebTokenService : IJsonWebToken {
		public string CreateRefreshToken(IConfiguration configuration)
		{
			var randomNumber = new byte[32];

			using(var rng = RandomNumberGenerator.Create())
			{
				rng.GetBytes(randomNumber);
				return Convert.ToBase64String(randomNumber);
			}
		}

		public string CreateToken(IConfiguration configuration, IEnumerable<Claim> claim)
		{
			SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecurityKey"]));

			SigningCredentials signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

			JwtSecurityToken jwt = new JwtSecurityToken(
				issuer: configuration["Jwt:Issuer"],
				audience: configuration["Jwt:Audience"],
				signingCredentials: signingCredentials,
				notBefore: DateTime.Now,
				claims: claim,
				expires: DateTime.Now.AddMinutes(Double.Parse(configuration["Jwt:Expiration"]))
			);
			return new JwtSecurityTokenHandler().WriteToken(jwt);
		}

		public async Task<IEnumerable> VerifyToken(IConfiguration configuration, string token)
		{
			var jwtTokenHandler = new JwtSecurityTokenHandler();
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecurityKey"]));
			try
			{
				var jwt = await jwtTokenHandler.ValidateTokenAsync(
					token, 
					new TokenValidationParameters()
					{
						ValidateAudience = true,
						ValidateIssuer = true,
						ValidateIssuerSigningKey = true,
						ValidateLifetime = true,
						ValidIssuer = configuration["Jwt:Issuer"],
						ValidAudience = configuration["Jwt:Audience"],
						IssuerSigningKey = securityKey
					}
				);
				return jwt.Claims;
			}
			catch (Exception ex)
			{
				return Array.Empty<string>();
			}
		}
	}
}
