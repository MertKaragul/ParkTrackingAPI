using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using ParkTracking.Models;
using ParkTracking.Services.Json_web_token;
using ParkTracking.Services.Managments.ParkManagement;
using ParkTracking.Services.Managments.RefreshToken;
using ParkTracking.Services.Managments.UserManagement;
using System.Diagnostics;
using System.Security.Claims;

namespace ParkTracking.Controllers
{

    [Route("/admin/[controller]")]
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
			var refreshToken = "";
			// Refresh token
			var refreshTokenValidity = refreshTokenManager.CheckTokenValidity(findUserModel.UserID);

			if(!refreshTokenValidity)
			{
				// Token expired, create new token
				refreshTokenManager.RemoveUserRefreshToken(findUserModel.UserID);
				refreshToken = _jsonWebTokenService.CreateRefreshToken(_configuration);
				refreshTokenManager.AddUserRefreshToken(findUserModel.UserID, refreshToken);
			}
			else
			{
				// Token not expired, get database token
				refreshToken = refreshTokenManager.GetUserRefreshToken(findUserModel.UserID);
			}

			return Ok( new { accessToken, refreshToken });
		}



		[HttpPost("/createUser")]
		[Authorize(Roles = "ADMIN")]
		public async Task<ActionResult> CreateUser([FromBody] UserModel? userModel)
		{
			if(userModel == null) return BadRequest(new { message = "User information is required" });
			try
			{
				UserManagement userManagement = new UserManagement(_configuration);
				userManagement.insert(userModel);
			}
			catch(Exception ex) {
				return StatusCode(500, "Server error");
			}

            return Ok(new { message = "User successfully created" });
		}

		[HttpPost("/createPark")]
		[Authorize(Roles = "ADMIN")]
		public async Task<ActionResult> CreatePark(string? identyNumber, [FromBody] CarModel? carModel)
		{
			if(identyNumber == null) return BadRequest(new { message = "Identy number is required" });

			UserManagement userManagement = new UserManagement(_configuration);
			ParkManagment parkManagement = new ParkManagment(_configuration);
			var findUser = await userManagement.findUserByIdentyNumber(identyNumber);
			if(carModel == null) return BadRequest(new { message = "Car required" });
			if(findUser == null) return NotFound(new { message = "User not found"});
			carModel.UserID = findUser.UserID;

			// Create Park area
			var random = new Random();

		





			return Ok(new { message = "Car area created" });
		}
	}
}
