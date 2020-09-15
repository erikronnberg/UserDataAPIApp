using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using UserDataAPIApp.Models;

namespace UserDataAPIApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<Account> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;
        private readonly string typeSchema = @"http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

        public UserController(UserManager<Account> _userManager, RoleManager<IdentityRole> _roleManager, IConfiguration _configuration, IMapper _mapper)
        {
            userManager = _userManager;
            roleManager = _roleManager;
            configuration = _configuration;
            mapper = _mapper;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUser model)
        {
            var userExists = await userManager.FindByNameAsync(model.Username);

            if (userExists != null)
                return StatusCode(StatusCodes.Status400BadRequest, new StatusResponse { Status = "Error", Message = "User already exists with that username!" });

            Account user = new Account()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                SocialSecurityNumber = long.Parse(model.SocialSecurityNumber)
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new StatusResponse { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            return Ok(new StatusResponse { Status = "Success", Message = "User created successfully!" });
        }

        [Authorize(Roles = Policies.Admin)]
        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterUser model)
        {
            var userExists = await userManager.FindByNameAsync(model.Username);

            if (userExists != null)
                return StatusCode(StatusCodes.Status400BadRequest, new StatusResponse { Status = "Error", Message = "User already exists!" });

            Account user = new Account()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                SocialSecurityNumber = long.Parse(model.SocialSecurityNumber)
            };
            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new StatusResponse { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            if (!await roleManager.RoleExistsAsync(Policies.Admin))
                await roleManager.CreateAsync(new IdentityRole(Policies.Admin));

            if (!await roleManager.RoleExistsAsync(Policies.User))
                await roleManager.CreateAsync(new IdentityRole(Policies.User));

            if (await roleManager.RoleExistsAsync(Policies.Admin))
                await userManager.AddToRoleAsync(user, Policies.Admin);

            return Ok(new StatusResponse { Status = "Success", Message = "User created successfully!" });
        }

        [Authorize(Roles = Policies.Admin)]
        [HttpGet]
        [Route("get-all-users")]
        public async Task<IActionResult> GetAll()
        {
            var users = await userManager.Users.ToListAsync();
            if (users == null)
                return StatusCode(StatusCodes.Status404NotFound, new StatusResponse { Status = "Error", Message = "No users found!" });

            var mappedResult = mapper.Map<UserResponse>(users);
            return Ok(mappedResult);
        }

        [Authorize]
        [HttpGet]
        [Route("get-single-user")]
        public async Task<IActionResult> GetUser(string username)
        {
            var userToGet = await userManager.FindByNameAsync(username);
            var user = Request.HttpContext.User;

            if (userToGet == null)
                return StatusCode(StatusCodes.Status400BadRequest, new StatusResponse { Status = "Error", Message = "" });

            if (user.Identity.Name != userToGet.UserName && user.Claims.Where(x => x.Type == typeSchema).Any(x => x.Value == "Admin") == false)
                return StatusCode(StatusCodes.Status401Unauthorized, new StatusResponse { Status = "Error", Message = "Unathorized to get this user!" });

            var mappedResult = mapper.Map<UserResponse>(userToGet);

            return Ok(mappedResult);
        }

        [Authorize]
        [HttpPatch]
        [Route("update")]
        public async Task<IActionResult> Update(string username, [FromBody] UpdateUser model)
        {
            var userToUpdate = await userManager.FindByNameAsync(username);
            var user = Request.HttpContext.User;

            if (userToUpdate == null)
                return StatusCode(StatusCodes.Status400BadRequest, new StatusResponse { Status = "Error", Message = "" });

            if (user.Identity.Name != userToUpdate.UserName && user.Claims.Where(s => s.Type == typeSchema).Any(s => s.Value == "Admin") == false)
                return StatusCode(StatusCodes.Status401Unauthorized, new StatusResponse { Status = "Error", Message = "Unathorized to update this user!" });

            var mappedResult = mapper.Map<UpdateUser, Account>(model, userToUpdate);

            var result = await userManager.UpdateAsync(mappedResult);

            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new StatusResponse { Status = "Error", Message = "User update failed! Please check user details and try again." });

            return Ok(new StatusResponse { Status = "Success", Message = "User updated successfully!" });
        }

        [Authorize]
        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> Delete(string username)
        {
            var userToDelete = await userManager.FindByNameAsync(username);
            var user = Request.HttpContext.User;

            if (userToDelete == null)
                return StatusCode(StatusCodes.Status400BadRequest, new StatusResponse { Status = "Error", Message = "" });

            if (user.Identity.Name != userToDelete.UserName && user.Claims.Where(s => s.Type == typeSchema).Any(s => s.Value == "Admin") == false)
                return StatusCode(StatusCodes.Status401Unauthorized, new StatusResponse { Status = "Error", Message = "Unathorized to delete this user!" });

            var result = await userManager.DeleteAsync(userToDelete);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new StatusResponse { Status = "Error", Message = "User deletion failed! Please check user details and try again." });

            return Ok(new StatusResponse { Status = "Success", Message = "User deleted successfully!" });
        }
    }
}
