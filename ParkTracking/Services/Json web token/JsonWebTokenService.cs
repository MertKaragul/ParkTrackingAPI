using Microsoft.IdentityModel.Tokens;
using System.Collections;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ParkTracking.Services.Json_web_token {
	// eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiVEVYQVNUNSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWVpZGVudGlmaWVyIjoiMSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFETUlOIiwibmJmIjoxNzAzMDk3NjM5LCJleHAiOjE3MDMwOTgyMzksImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3QiLCJhdWQiOiJodHRwOi8vbG9jYWxob3N0In0.AfUdwKKOHU65EPZJ6ttD9-g_D5UPWYW9n75YV2-VhpM
	// w12+5TRZeuv6ECQ1fxnOWVS4cFfa/uQT3arXwQf/3zA=

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

		public async Task<bool> VerifyToken(IConfiguration configuration, string token)
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

				return true;
            }
			catch (Exception ex)
			{
				return false;
			}
		}
	}
}
