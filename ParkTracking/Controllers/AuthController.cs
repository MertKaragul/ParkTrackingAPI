using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ParkTracking.Models;
using ParkTracking.Services.Json_web_token;
using ParkTracking.Services.Managments.RefreshToken;
using ParkTracking.Services.Managments.UserManagement;
using System.Security.Claims;

namespace ParkTracking.Controllers
{
    [Route("/auth/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private JsonWebTokenService _jsonWebTokenService;
        private RefreshTokenManager _refreshTokenManager;
        private UserManagement _userManagement;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
            _jsonWebTokenService = new JsonWebTokenService();
            _refreshTokenManager = new RefreshTokenManager(configuration);
            _userManagement = new UserManagement(configuration);
        }

        [HttpPost("/login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login(string? identyNumber, string? password)
        {
            if (
                Empty.Equals(password) ||
                password == null ||
                identyNumber == null ||
                Empty.Equals(identyNumber)
                ) return BadRequest(new { message = "Information cannot be empty, check your IdentyNumber,Password input values" });

            UserModel? findUserModel = await _userManagement.findUserByIdentyNumber(identyNumber);
            if (findUserModel == null) return NotFound(new { message = "User not found" });
            if (
                findUserModel.IdentyNumber.Trim() != identyNumber.Trim() ||
            findUserModel.Password.Trim() != password.Trim()
            ) return NotFound(new { message = "Your IdentyNumber or Password wrong please try again" });

            var accessToken = _jsonWebTokenService.CreateToken(_configuration, findUserModel.toClaim());
            var refreshToken = "";
            // Refresh token
            var refreshTokenValidity = _refreshTokenManager.CheckTokenValidity(findUserModel.UserID);

            if (!refreshTokenValidity)
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

            return Ok(new { accessToken, refreshToken });
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
            if (getTokenModel == null)
            {
                return Unauthorized();
            }

            UserManagement userManagement = new UserManagement(_configuration);
            var findUser = await userManagement.findUserById(getTokenModel.UserId);
            var access_token = _jsonWebTokenService.CreateToken(_configuration, findUser.toClaim());

            return Ok(new { access_token });
        }



    }
}
