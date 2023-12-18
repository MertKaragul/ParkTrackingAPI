﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ParkTracking.Models;
using ParkTracking.Services.Json_web_token;
using ParkTracking.Services.Managments.RefreshToken;
using ParkTracking.Services.Managments.UserManagement;
using System.Security.Claims;

namespace ParkTracking.Controllers
{

    [Route("/[controller]")]
	[ApiController]
	public class AdminController : ControllerBase {

		private readonly IConfiguration _configuration;
		private JsonWebTokenService _jsonWebTokenService;

		public AdminController(IConfiguration configuration)
		{
			_configuration = configuration;
			_jsonWebTokenService = new JsonWebTokenService();
		}


		[HttpPost("/adminLogin")]
		[AllowAnonymous]
		public async Task<ActionResult> AdminLogin(string? username, string? password,string? identyNumber)
		{
			if(
				Empty.Equals(username) ||
				username == null ||
				Empty.Equals(password) ||
				password == null ||
				identyNumber == null ||
				Empty.Equals(identyNumber)
				) return BadRequest( new { message = "Information cannot be empty, check your Name,IdentyNumber,Password input values" });

			UserManagement userManagement = new UserManagement(_configuration);
			RefreshTokenManager refreshTokenManager = new RefreshTokenManager(_configuration);

			UserModel? findUserModel = await userManagement.findUserByIdentyNumber(identyNumber);
			if(findUserModel == null) return NotFound(new { message = "User not found" });
			if(
				findUserModel.Name.Trim() != username.Trim() ||
				findUserModel.IdentyNumber.Trim() != identyNumber.Trim() ||
				findUserModel.Password.Trim() != password.Trim()
				) return NotFound(new { message = "Your Name,IdentyNumber or Password wrong please try again" });
			if(findUserModel.Roles == Roles.USER) return BadRequest(new { message = "Just only Admin log-in" });

			var claim = new[] { 
				new Claim(ClaimTypes.Name, username),
				new Claim(ClaimTypes.NameIdentifier, findUserModel.UserID.ToString()),
				new Claim(ClaimTypes.Role, findUserModel.Roles.ToString()),
			};

			var accessToken = _jsonWebTokenService.CreateToken(_configuration,claim);
			var refreshToken = _jsonWebTokenService.CreateRefreshToken(_configuration);
			refreshTokenManager.AddUserRefreshToken(findUserModel.UserID, refreshToken);
			return Ok( new { accessToken,refreshToken });
		}

	}
}