using Microsoft.AspNetCore.Mvc;  
using OnlineEnrollmentSystem.Models;  

namespace OnlineEnrollmentSystem.Controllers  
{  
    public class HomeController : Controller  
    {  

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
        public IActionResult Login(UserModel loginRequest)  
        {  
            // For simplicity, just validate hardcoded credentials for now
            if (loginRequest.Username == "student" && loginRequest.Password == "password")  
            {  
                // Store user session (demo only, no real authentication yet)  
                TempData["UserName"] = loginRequest.Username;  
                return RedirectToAction("Courses");  
            }  

            ModelState.AddModelError(string.Empty, "Invalid credentials.");  
            return View();  
        }  

        // GET: /Home/Courses  
        public IActionResult Courses()  
        {  
            // Mock API response for available courses  
            List<CourseModel> courses = new List<CourseModel>  
            {  
                new CourseModel { Id = 1, Name = "ST-MATH", IsEnrolled = false },  
                new CourseModel { Id = 2, Name = "ST-INTSY", IsEnrolled = true }  
            };  
            return View(courses);  
        }  

        // GET: /Home/Grades  
        public IActionResult Grades()  
        {  
            // Mock grades for demonstration  
            List<GradeModel> grades = new List<GradeModel>  
            {  
                new GradeModel { CourseName = "ST-MATH", Grade = "3.0" },  
                new GradeModel { CourseName = "ST-INTSY", Grade = "2.5" }  
            };  
            return View(grades);  
        }  
    }  
}