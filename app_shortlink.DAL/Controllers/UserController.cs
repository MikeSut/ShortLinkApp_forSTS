using app_shortlink.DAL.Repository.IRepository;
using app_shortlink.Domain.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace app_shortlink.DAL.Controllers
{
    [ApiController]
    [Route("[controller]")]
    
    public class UserController: ControllerBase
    {
        private readonly IUserRepository _userRepo;
        

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var response = await _userRepo.Login(model);
            if (response.User == null || string.IsNullOrEmpty(response.Token))
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {
            var response = await _userRepo.Register(model);

            if (response == null)
            {
                return BadRequest(new { message = "Didn't register!" });
            }

            return Ok(response);
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userRepo.GetAll();
            return Ok(users);
        }

    }
    
    
    
}

