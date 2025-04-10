using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySqlConnector.Logging;
using OnlineEnrollmentSystem.Data;
using OnlineEnrollmentSystem.Models;
using OnlineEnrollmentSystem.Services;
using Org.BouncyCastle.Security;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineEnrollmentSystem.Controllers  
{
	public class HomeController : Controller
	{
		private readonly AppDbContext _context;
		private readonly IConfiguration _configuration;
		private readonly TokenService _tokenService;

		// Modify the constructor to accept IConfiguration and TokenService as dependencies
		public HomeController(AppDbContext context, IConfiguration configuration, TokenService tokenService)
		{
			_context = context;
			_configuration = configuration;
			_tokenService = tokenService;  // This is now correctly injected
		}

		public IActionResult Index()
		{
			return View();
		}

		// GET: /Home/Login
		public IActionResult Login()
		{
			return View();
		}

		// POST: /Home/Login
		[HttpPost]
		public async Task<IActionResult> Login(UserAuthModel loginRequest)
		{
			if (!ModelState.IsValid)
			{
				return View(loginRequest);
			}

			// Check for matching user in the database
			var user = await _context.Users
				.FirstOrDefaultAsync(u => u.Id == loginRequest.Id && u.Password == loginRequest.Password);

			if (user != null)
			{
				// Generate JWT Token using the injected TokenService
				var token = _tokenService.GenerateJwtToken(user.Id, user.Role.ToString());

				// Store the token in a cookie (for example)
				Response.Cookies.Append("jwt", token, new CookieOptions
				{
					HttpOnly = true,
					Expires = DateTime.UtcNow.AddDays(1)
				});

				// Optionally, store user role and ID in the session if needed
				HttpContext.Session.SetString("role", user.Role.ToString());
				HttpContext.Session.SetInt32("userId", user.Id);
				HttpContext.Session.SetString("username", user.Username);

				// Redirect to the Courses page
				return RedirectToAction("Index", "Courses");
			}

			ModelState.AddModelError(string.Empty, "Invalid credentials.");
			return View(loginRequest);
		}
	}
}