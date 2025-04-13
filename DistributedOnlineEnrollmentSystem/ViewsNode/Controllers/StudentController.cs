using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ViewsNode.Models;

namespace ViewsNode.Controllers
{
    public class StudentController : Controller
    {
        private readonly HttpClient _httpClient;

        public StudentController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("StudentApi");
        }

        [HttpGet("/Student/Grades")]
        public async Task<IActionResult> Grades()
        {
            var jwtToken = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(jwtToken))
                return RedirectToAction("Login", "Home");

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);

            //var response = await _httpClient.GetAsync("Student/Grades");
            var response = await _httpClient.GetAsync("http://studentnode:8080/api/student/grades");
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            var grades = JsonSerializer.Deserialize<List<EnrollmentModel>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(grades);
        }

        [HttpPost("/Student/Enroll")]
        public async Task<IActionResult> Enroll(int courseId)
        {
            var jwtToken = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(jwtToken))
                return RedirectToAction("Login", "Home");

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);

            var payload = new { CourseId = courseId };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            //var response = await _httpClient.PostAsync("Student/Enroll", content);
            var response = await _httpClient.PostAsync("http://studentnode:8080/api/student/enroll", content);

            Console.WriteLine("Student Lily:");
            Console.WriteLine(response);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Successfully enrolled!";
            }
            else
            {
                TempData["Error"] = "Enrollment failed.";
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}");
                Console.WriteLine($"Details: {errorContent}");
            }

            return RedirectToAction("Index", "Courses");
        }
    }
}
