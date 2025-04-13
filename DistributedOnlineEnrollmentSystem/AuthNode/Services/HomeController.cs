using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthNode.Models;
using AuthNode.Services;
using System.Threading.Tasks;

namespace AuthNode.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly TokenService _tokenService;

        public HomeController(AppDbContext context, IConfiguration configuration, TokenService tokenService)
        {
            _context = context;
            _configuration = configuration;
            _tokenService = tokenService;
        }

        [HttpGet("index")]
        public IActionResult Index()
        {
            return Ok("AuthNode is up and running.");
        }

        // POST: /api/home/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserAuthModel loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid login details");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == loginRequest.Id && u.Password == loginRequest.Password);

            if (user != null)
            {
                string role;
                if (!user.Role)
                {
                    role = "student";
                }
                else
                {
                    role = "instructor";
                }

                var token = _tokenService.GenerateJwtToken(user.Id, role);

                return Ok(new
                {
                    token,
                    userId = user.Id,
                    username = user.Username,
                    role = user.Role ? "instructor" : "student"
                });
            }

            return Unauthorized("Invalid credentials.");
        }

        // POST: /api/home/logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // API version: assume session clearing is handled on ViewsNode
            return Ok("Logged out.");
        }
    }
}
