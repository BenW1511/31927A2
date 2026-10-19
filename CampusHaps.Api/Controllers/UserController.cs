using CampusHaps.Api.DTOs;
using CampusHaps.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CampusHaps.Api.Controllers
{
    //handles requests from the frontend related to users
    //used for things like registering and finding users
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {

        private readonly IUserService _users;

        public UserController(IUserService users)
        {
            _users = users;
        }

        //get api/users
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_users.GetAll());
        }

        //get /api/users/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var user = _users.GetById(id);

            if(user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        //post /api/user/register
        [HttpPost("register")]
        public IActionResult Register(RegisterDto dto)
        {
            var user = _users.Register(dto);

            if(user == null)
            {
                return BadRequest("That email is already in use");
            }

            return Ok(user);
        }

        //post /api/users/login
        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            var user = _users.Login(dto);

            if( user == null)
            {
                return Unauthorized("Invalid email or password");
            }

            return Ok(user);
        }



    }
}
