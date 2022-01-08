using JwtAuthDemo.Methods;
using JwtAuthDemo.Models.Entities;
using JwtAuthDemo.Models.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JwtAuthDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private List<User> _userList = new List<User>()
        {
            new User {Id = 1, Name = "Mert Savaş", Email = "mert@gmail.com", Password = "123"},
            new User {Id = 2, Name = "John Doe", Email = "john@gmail.com", Password = "123"},
            new User {Id = 3, Name = "Chris Evans", Email = "chris@gmail.com", Password = "123"}
        };

        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            var user = _userList.FirstOrDefault(x => x.Email == model.Email && x.Password == model.Password);
            if (user == null)
                return NotFound();

            var token = new TokenOperations(_configuration).CreateToken(user.Email);
            return token != null ? Ok(token) : BadRequest();
        }

        [HttpGet("getUser")]
        [Authorize]
        public IActionResult GetUser()
        {
            var token = HttpContext.Request.Headers["Authorization"];
            var email = new TokenOperations(_configuration).DecodeToken(token);
            var user = _userList.FirstOrDefault(x => x.Email == email);
            return user != null ? Ok(user) : BadRequest();
        }
    }
}
