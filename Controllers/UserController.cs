using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using UserDataAPIApp.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UserDataAPIApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly Mapper mapper;

        public UserController (UserManager<User> _userManager, Mapper _mapper)
        {
            userManager = _userManager;
            mapper = _mapper;
        }

        // GET: api/<UserController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UserController>
        [HttpPost("Create")]
        public async Task<IdentityResult> CreateUser([FromBody] CreateRequest model)
        {
            try
            {
                var p = mapper.Map(model, User);
                var user = await userManager.CreateAsync();
                return user;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public async Task<bool> Delete(int id)
        {
            try
            {
                var user = await userManager.FindByIdAsync(id.ToString());
                await userManager.DeleteAsync(user);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
