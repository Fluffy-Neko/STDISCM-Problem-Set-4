using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ViewsNode.Models;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ViewsNode.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public HomeController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
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

            // Use HttpClient to send the credentials to the AuthNode
            var client = _httpClientFactory.CreateClient("AuthApi");

            var content = new StringContent(
                JsonSerializer.Serialize(loginRequest),
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync("home/login", content);
            Console.WriteLine($"{response}");

            if (response.IsSuccessStatusCode)
            {
                // Parse the JWT token from the response
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<JsonElement>(jsonResponse);
                var token = responseObject.GetProperty("token").GetString();

                // Store the token in the session or a cookie
                HttpContext.Session.SetString("JwtToken", token);

                // Redirect to the Courses page (or wherever you want)
                return RedirectToAction("Index", "Courses");
            }
            else
            {
                // Handle error (invalid credentials)
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(loginRequest);
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Home");
        }
    }
}
