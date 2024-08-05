using Microsoft.AspNetCore.Mvc;
using Fyp.Models;
using WebApplication8.Models;

namespace Fyp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserContext _userContext;

        public UsersController(UserContext userContext)
        {
            _userContext = userContext;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                bool isRegistered = _userContext.RegisterUser(model.FirstName, model.LastName, model.Email, model.Password);

                if (isRegistered)
                {
                    return Ok(new { message = "User registered successfully" });
                }
                else
                {
                    return StatusCode(500, "An error occurred while registering the user");
                }
            }

            return BadRequest(ModelState);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Login model)
        {
            if (ModelState.IsValid)
            {
                bool isValidUser = _userContext.ValidateUser(model.Email, model.Password);

                if (isValidUser)
                {
                    return Ok(new { message = "User logged in successfully" });
                }
                else
                {
                    return Unauthorized(new { message = "Invalid email or password" });
                }
            }

            return BadRequest(ModelState);
        }
    }
}
