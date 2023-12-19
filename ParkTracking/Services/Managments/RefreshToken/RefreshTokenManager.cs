using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using ParkTracking.Models;
using ParkTracking.Services.Database;
using System.Diagnostics;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;

namespace ParkTracking.Services.Managments.RefreshToken {
	public class RefreshTokenManager {

		private string TIME_PATTERN = "yyyy.MM.dd HH:mm:ss";
		
		private readonly IConfiguration _configuration;
		public RefreshTokenManager(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public void AddUserRefreshToken(int userId, string token)
		{
			using var context = new Context(_configuration);
			RefreshTokenModel refreshTokenModel = new RefreshTokenModel()
			{
				UserId = userId,
				RefreshToken = token,
				StartTime = DateTime.Now.ToString(TIME_PATTERN),
				EndTime = DateTime.Now.AddMonths(1).ToString(TIME_PATTERN),
			};
			context.Add(refreshTokenModel);
			context.SaveChanges();
		}

		public bool CheckTokenValidity(int userID)
		{
			// If function return true, token not expired
			// If function return false, token expired
			using var context = new Context(_configuration);
			var findUserToken = context.Set<RefreshTokenModel>().FirstOrDefault(x => x.UserId == userID) ?? null;

			if(findUserToken == null) return false;

			
			if(DateTime.TryParseExact(findUserToken.EndTime, TIME_PATTERN, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDateTime))
			{
				Debug.WriteLine("Parsed date time : " + parsedDateTime + " Date time Now " + DateTime.Now);
				if(parsedDateTime < DateTime.Now)
				{
					// Token not expired
					return true;
				}
				else
				{
					return false;
				}
			}
			return true;
		}

		public void RemoveUserRefreshToken(int userId)
		{
			using var context = new Context(_configuration);
			context.Remove(userId);
			context.SaveChanges();
		}

		public string? GetUserRefreshToken(int userID)
		{
			using var context = new Context(_configuration);
			var refreshToken = context.Set<RefreshTokenModel>().FirstOrDefault(x => x.UserId == userID);
			return refreshToken == null ? null : refreshToken.RefreshToken;
		}

		
	}
}
