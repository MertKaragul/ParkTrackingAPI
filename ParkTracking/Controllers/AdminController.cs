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
		private RefreshTokenManager _refreshTokenManager;
		private UserManagement _userManagement;

		public AdminController(IConfiguration configuration)
		{
			_configuration = configuration;
			_jsonWebTokenService = new JsonWebTokenService();
			_refreshTokenManager = new RefreshTokenManager(configuration);
			_userManagement = new UserManagement(configuration);
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

			UserModel? findUserModel = await _userManagement.findUserByIdentyNumber(identyNumber);
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
			var refreshTokenValidity = _refreshTokenManager.CheckTokenValidity(findUserModel.UserID);

			if(!refreshTokenValidity)
			{
                // Token expired, create new token
                _refreshTokenManager.RemoveUserRefreshToken(findUserModel.UserID);
				refreshToken = _jsonWebTokenService.CreateRefreshToken(_configuration);
                _refreshTokenManager.AddUserRefreshToken(findUserModel.UserID, refreshToken);
			}
			else
			{
				// Token not expired, get database token
				refreshToken = _refreshTokenManager.GetUserRefreshToken(findUserModel.UserID);
			}

			return Ok( new { accessToken, refreshToken });
		}


        [HttpPost("/refresh")]
        [AllowAnonymous]
        public async Task<ActionResult> RefreshToken(string? refreshToken)
        {
			if (refreshToken == null) return BadRequest();
			var tokenValidity = _refreshTokenManager.CheckTokenValidity(refreshToken);
			if (!tokenValidity)
			{
				return Unauthorized();
			}

			// Create new access token
			var getTokenModel = _refreshTokenManager.getTokenModelByRefreshToken(refreshToken);
			if (getTokenModel == null) return Unauthorized();

			UserManagement userManagement = new UserManagement(_configuration);
			var findUser = await userManagement.findUserById(getTokenModel.UserId);
			var access_token = _jsonWebTokenService.CreateToken(_configuration, findUser.toClaim());
			

            return Ok(new { access_token });
        }



        [HttpPost("/createUser")]
		[Authorize(Roles = "ADMIN")]
		public async Task<ActionResult> CreateUser([FromBody] UserModel? userModel)
		{
			if(userModel == null) return BadRequest(new { message = "User information is required" });
			try
			{

				if(_userManagement.findUserByIdentyNumber(userModel.IdentyNumber) != null)
				{
					return BadRequest(new { message = "User already exits" });
				}
                _userManagement.insert(userModel);
			}
			catch(Exception ex) {
				return StatusCode(500, "Server error");
			}

            return Ok(new { message = "User successfully created" });
		}

		[HttpPost("/createPark")]
		[Authorize(Roles = "ADMIN")]
		public async Task<ActionResult> CreatePark(string? identyNumber)
		{
			if (identyNumber == null) return BadRequest(new { message = "Identy number required" });
			


			return Ok(new { message = "Park created" });
		}
	}
}
