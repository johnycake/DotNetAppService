using DotNetCoreApp1.Controllers.Requests;
using DotNetCoreApp1.Models;
using DotNetCoreApp1.Models.Interfaces;
using DotNetCoreApp1.Models.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DotNetCoreApp1.Controllers
{
    [Route("api/authenticanion")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthenticationController(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));   
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult<string>> Authenticate(AuthenticationRequestBody authenticationRequestBody)
        {
            var user = await ValidateUserCredentials(authenticationRequestBody.UserName, authenticationRequestBody.Password);
            if (user == null)
            {
                return Unauthorized();
            }

            var token = await GenerateJwtTokenAsync(user);

            if (token == null)
            {  
                return Problem("Validation Problem"); 
            }

            return Ok(token);
        }

        private async Task<UserDto?> ValidateUserCredentials(string? userName, string? password)
        {
            return await _userRepository.GetUserByCredentials(userName, password);
        }

        private Task<string?> GenerateJwtTokenAsync(UserDto user) 
        {
            return Task.FromResult(GenerateJwtToken(user));
        }

        private string? GenerateJwtToken(UserDto user)
        {
            var secretForKey = _configuration["Authentication:SecretForKey"];

            if (secretForKey == null)
            {
                return null;
            }

            var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(secretForKey));

            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("sub", user.UserId.ToString()));
            claimsForToken.Add(new Claim("role_Id", user.RoleId ?? ""));

            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials
                );

            var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return tokenToReturn;
        }
    }
}
