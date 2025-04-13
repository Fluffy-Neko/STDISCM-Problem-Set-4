using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ViewsNode.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace ViewsNode.Controllers
{
    public class CoursesController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CoursesController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");
            var jwtToken = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(jwtToken))
            {
                return RedirectToAction("Login", "Home");
            }

            var client = _httpClientFactory.CreateClient("BasicFacilitiesApi");

            // Set the Authorization header with the JWT token
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await client.GetAsync("/Courses");
            Console.WriteLine("LILY");
            Console.WriteLine(response);

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login", "Home");
            }

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var viewModel = JsonSerializer.Deserialize<CourseListViewModel>(json, options);
            var debugJson = JsonSerializer.Serialize(viewModel, new JsonSerializerOptions { WriteIndented = true });

            Console.WriteLine(json);
 
            ViewBag.UserRole = role;
            return View(viewModel);
        }
    }
}
