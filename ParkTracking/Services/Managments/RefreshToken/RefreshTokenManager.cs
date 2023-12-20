using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using ParkTracking.Models;
using ParkTracking.Services.Database;
using System.Diagnostics;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;

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

		public bool CheckTokenValidity(int userID)
		{
			// If function return true, token not expired
			// If function return false, token expired
			using var context = new Context(_configuration);
			var findUserToken = context.Set<RefreshTokenModel>().FirstOrDefault(x => x.UserId == userID) ?? null;

			if(findUserToken == null) return false;

			return DateTime.Now < findUserToken.EndTime;
		}

		public bool CheckTokenValidity(string refreshToken)
		{
			// If function return true, token not expired
			// If function return false, token expired
			using var context = new Context(_configuration);
			var findUserToken = context.Set<RefreshTokenModel>().FirstOrDefault(x => x.RefreshToken == refreshToken) ?? null;

			if(findUserToken == null) return false;

			return DateTime.Now < findUserToken.EndTime;
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
