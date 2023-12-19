using System.Net;
using app_shortlink.DAL.Repository.IRepository;
using app_shortlink.Domain.Dto;
using Microsoft.AspNetCore.Mvc;

namespace app_shortlink.DAL.Controllers
{
    [ApiController]
    [Route("api/userAuth")]
    
    
    public class UserController: ControllerBase
    {
        private readonly IUserRepository _userRepo;
        protected APIResponse _response;

        public UserController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
            this._response = new();
        }
        

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var loginResponse = await _userRepo.Login(model);
            if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSucces = false;
                _response.ErrorMessages.Add("Username or password is incorrect");
                return BadRequest(_response);
            }
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSucces = true;
            _response.ErrorMessages.Add("Username or password is incorrect");
            return Ok(_response);
        }

        [HttpPost("register")]
        public async Task<IActionResult>  Register([FromBody] RegistrationRequestDto model)
        {
            bool ifUserNameUnique = _userRepo.IsUniqueUser(model.UserName);
            if (!ifUserNameUnique)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSucces = false;
                _response.ErrorMessages.Add("Username already exists");
                return BadRequest(_response);
            }

            var user = await _userRepo.Register(model);
            if (user == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSucces = false;
                _response.ErrorMessages.Add("Error while register");
                return BadRequest(_response);
            }

            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSucces = true;
            return Ok(_response);
        }


    }
    
    
    
}

