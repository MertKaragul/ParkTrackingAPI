using Microsoft.EntityFrameworkCore;
using ParkTracking.Models;
using ParkTracking.Services.Database;

namespace ParkTracking.Services.Managments.RefreshToken {
	public class RefreshTokenManager {
		
		private readonly IConfiguration _configuration;
		public RefreshTokenManager(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public void AddUserRefreshToken(int userId, string token)
		{
			using var context = new Context(_configuration);
			if(CheckTokenValidity(userId))
			{
				RefreshTokenModel refreshTokenModel = new RefreshTokenModel()
				{
					UserId = userId,
					RefreshToken = token,
					StartTime = DateTime.Now,
					EndTime = DateTime.Now.AddMonths(1),
				};
				context.Add(refreshTokenModel);
				context.SaveChanges();
			}
		}

		public bool CheckTokenValidity(int userID)
		{
			// If function return true, token validity expired
			// If function return false, token validity continued
			using var context = new Context(_configuration);
			var findUserToken = context.Set<RefreshTokenModel>().FirstOrDefault(x => x.UserId == userID);
			try
			{
				if(findUserToken == null) return true;

				if(findUserToken.EndTime <= DateTime.Now)
				{
					return false;
				}
				return true;
				
			}catch (Exception ex)
			{
				return true;
			}
		}

		public void RemoveUserRefreshToken(int userId)
		{
			using var context = new Context(_configuration);
			context.Remove(userId);
			context.SaveChanges();
		}

		
	}
}
