using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;

namespace WebCodeFirstODataMySQL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            try
            {


                if (login.Username == "abinash" && login.Password == "1234") 
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes("4cCI6MTcyNDQyNzE0OCwiaWF0IjoxNzI0NDIzNTQ4fQ.LeVX7Z7__frSIH7vUuYUCInJ2aYZCc8A2GvS1NecIak"); 
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.Name, login.Username)
                    }),
                        Expires = DateTime.UtcNow.AddMinutes(15),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    _logger.LogInformation("User '{Username}' logged in successfully.", login.Username);
                    return Ok(new { Token = tokenHandler.WriteToken(token) });
                }
                _logger.LogWarning("Unauthorized login attempt for user '{Username}'.", login.Username);
                return Unauthorized();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login for user '{Username}'.", login.Username);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error. Please try again later.");
            }



        }
    }

    public class LoginModel
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
