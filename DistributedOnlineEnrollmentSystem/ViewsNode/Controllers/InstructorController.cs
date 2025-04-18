﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ViewsNode.Models;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Text;

namespace ViewNodes.Controllers
{
    public class InstructorController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public InstructorController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> View(int id)
        {
            var jwt = HttpContext.Session.GetString("JwtToken");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(jwt) || role != "instructor")
                return RedirectToAction("Login", "Home");

            var client = _httpClientFactory.CreateClient("InstructorApi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            //var response = await client.GetAsync($"/api/Instructor/View?id={id}");
            var response = await client.GetAsync($"http://instructornode:8080/api/Instructor/View?id={id}");
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Sensei Lily:");
            Console.WriteLine(response);
            Console.WriteLine($"id: {id}");
            Console.WriteLine($"Error: {response.StatusCode}");
            Console.WriteLine($"Details: {errorContent}");
            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Index", "Courses");

            var json = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(json);
            Console.WriteLine($"json: {json}");

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var viewModel = JsonSerializer.Deserialize<CourseViewModel>(json, options);
            Console.WriteLine($"viewModel: {viewModel}");

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateGrade(int studentId, int courseId, string grade)
        {
            var jwt = HttpContext.Session.GetString("JwtToken");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(jwt) || role != "instructor")
                return RedirectToAction("Login", "Home");

            var client = _httpClientFactory.CreateClient("InstructorApi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            var payload = new
            {
                studentId,
                courseId,
                grade
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            //var response = await client.PostAsync("Instructor/UpdateGrade", content);
            var response = await client.PostAsync("http://instructornode:8080/api/instructor/updategrade", content);
            Console.WriteLine("Sensei Lily:");
            Console.WriteLine(response);

            return RedirectToAction("View", new { id = courseId });
        }
    }
}