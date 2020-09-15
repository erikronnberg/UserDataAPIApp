using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserDataAPIApp.Models;

namespace UserDataAPIApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;

        public UserController(UserManager<User> _userManager, RoleManager<IdentityRole> _roleManager, IConfiguration _configuration, IMapper _mapper)
        {
            userManager = _userManager;
            roleManager = _roleManager;
            configuration = _configuration;
            mapper = _mapper;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] Register model)
        {
            var userExists = await userManager.FindByNameAsync(model.Username);

            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            User user = new User()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

        [Authorize(Policies.Admin)]
        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] Register model)
        {
            var userExists = await userManager.FindByNameAsync(model.Username);

            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            User user = new User()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                SocialSecurityNumber = int.Parse(model.SSN)
            };
            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            if (!await roleManager.RoleExistsAsync(Policies.Admin))
                await roleManager.CreateAsync(new IdentityRole(Policies.Admin));

            if (!await roleManager.RoleExistsAsync(Policies.User))
                await roleManager.CreateAsync(new IdentityRole(Policies.User));

            if (await roleManager.RoleExistsAsync(Policies.Admin))
                await userManager.AddToRoleAsync(user, Policies.Admin);

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

        [Authorize(Roles = Policies.Admin)]
        [HttpGet]
        [Route("all-users")]
        public async Task<IActionResult> GetAll()
        {
            var users = await userManager.Users.ToListAsync();
            if (users == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Failed to get users! Please check user details and try again." });

            var mappedResult = mapper.Map<IEnumerable<GetAll>>(users);
            return Ok(mappedResult);
        }

        [Authorize]
        [HttpGet]
        [Route("get-user")]
        public async Task<IActionResult> GetUser([FromBody] GetUser model)
        {
            var userToGet = await userManager.FindByNameAsync(model.Username);
            var user = Request.HttpContext.User;

            if (user.Identity.Name != userToGet.UserName && user.Claims.Where(x => x.Type == "Role").Any(x => x.Value == "Admin") == false)
                return StatusCode(StatusCodes.Status401Unauthorized, new Response { Status = "Error", Message = "Unathorized to get this user!" });

            if (userToGet == null)
                return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "No user was found!" });

            return Ok(userToGet);
        }

        [Authorize]
        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> Delete([FromBody] Delete model)
        {
            var userToDelete = await userManager.FindByIdAsync(model.id);
            var user = Request.HttpContext.User;

            if (user.Identity.Name != userToDelete.UserName && user.Claims.Where(s => s.Type == "Role").Any(s => s.Value == "Admin") == false)
                return StatusCode(StatusCodes.Status401Unauthorized, new Response { Status = "Error", Message = "Unathorized to delete this user!" });

            if (userToDelete == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User does not exists!" });

            var result = await userManager.DeleteAsync(userToDelete);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User deletion failed! Please check user details and try again." });

            return Ok(new Response { Status = "Success", Message = "User deleted successfully!" });
        }
    }
}
