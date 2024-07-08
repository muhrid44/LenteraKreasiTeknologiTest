using Identity.Helper;
using Identity.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Identity.Controllers
{
    public class IdentityController : Controller
    {
        readonly IConfiguration _config;
        readonly DateTime _tokenLifetime = DateTime.Now.AddHours(1);
        readonly IUserService _userService;

        public IdentityController(IConfiguration config, IUserService userService)
        {
            _config = config;
            _userService = userService;
        }

        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] SignUpModel model)
        {
            try
            {
                PasswordUtility.PasswordHashGenerator(model.Password, out byte[] passwordHash, out byte[] passwordSalt);
                var newUser = new UserAuthModel
                {
                    Name = model.Name,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt
                };

                var result = await _userService.SignUp(newUser);
                if(result == "Ok")
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> GenereateTokenAsync([FromBody] LoginModel model)
        {

            (string, bool) jwt = await _userService.Login(model);
            if (jwt.Item2)
            {
                return Ok(jwt.Item1);
            }

            return BadRequest(jwt.Item1);
        }
    }
}
