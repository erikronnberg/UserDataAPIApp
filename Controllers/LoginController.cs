using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UserDataAPIApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;

namespace JWTAuthenticationExample.Controllers
{
[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;
        private UserManager<User> userManager;

        public LoginController(IConfiguration config)
        {
            _config = config;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] string userName, string password)
        {
            IActionResult response = Unauthorized();
            User user = await AuthenticateUser(userName, password);

            if (user != null)
            {
                var tokenString = GenerateJWTToken(user);
                response = Ok(new { token = tokenString });
            }

            return response;
        }

        private string GenerateJWTToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt: SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.UserName),
                new Claim("SSN", userInfo.SocialSecurityNumber.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, userInfo.Email.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
};
            var token = new JwtSecurityToken(
            issuer: _config["Jwt: Issuer"],
            audience: _config["Jwt: Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(120),
            signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        
        private async Task<User> AuthenticateUser(string username, string password)
        {
            var _user = await userManager.FindByIdAsync(username);
            var isPasswordOk = await userManager.CheckPasswordAsync(_user, password);
            if (isPasswordOk == false)
            {
                return null;
            }
            return _user;
        }
    }
}