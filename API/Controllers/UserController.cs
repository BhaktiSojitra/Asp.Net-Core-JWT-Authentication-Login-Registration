using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        public UserController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        #region GetAllUsers
        [HttpGet("GetAll")]
        public IActionResult GetAllUsers()
        {
            var users = _userRepository.SelectAll();
            return Ok(new { Data = users, Message = "Successfully retrieved users!" });
        }
        #endregion

        #region SignUpUser
        [HttpPost("SignUpUser")]
        public IActionResult SignUpUser([FromBody] UserModel userModel)
        {
            if (userModel == null)
            {
                return BadRequest(new { Message = "Invalid user data." });
            }

            string errorMessage;
            bool isInserted = _userRepository.Signup(userModel, out errorMessage);

            if (!isInserted)
            {
                return BadRequest(new { Message = errorMessage });
            }

            return Ok(new { Message = "User registered successfully!" });
        }
        #endregion

        #region LoginUser
        [HttpPost("LoginUser")]
        public IActionResult LoginUser([FromBody] LoginModel loginModel)
        {
            if (loginModel == null)
            {
                return BadRequest(new { Message = "Invalid login data." });
            }

            string errorMessage;
            string token = _userRepository.Login(loginModel, out errorMessage);

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { Message = errorMessage });
            }

            return Ok(new
            {
                Token = token,
                Message = "Login successfully!"
            });
        }
        #endregion
    }
}
