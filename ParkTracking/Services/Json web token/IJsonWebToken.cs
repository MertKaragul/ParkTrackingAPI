using System.Collections;
using System.Security.Claims;

namespace ParkTracking.Services.Json_web_token {
	public interface IJsonWebToken {
		string CreateToken(IConfiguration configuration, IEnumerable<Claim> claim);
		string CreateRefreshToken(IConfiguration configuration);
		Task<IEnumerable> VerifyToken(IConfiguration configuration, string token);
	}
}
