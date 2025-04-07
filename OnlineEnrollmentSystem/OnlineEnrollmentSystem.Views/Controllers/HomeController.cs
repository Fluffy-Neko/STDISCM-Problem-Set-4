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
				return RedirectToAction("View", "Courses");  
			}  

            ModelState.AddModelError(string.Empty, "Invalid credentials.");  
            return View();  
        }  

        // POST: /Home/Logout  
        [HttpPost]  
        public IActionResult Logout()  
        {  
            // Clear any user session data stored in TempData (if used)  
            TempData.Clear();  

            // Redirect to the Login page  
            return RedirectToAction("Login");  
        }  

		// GET: /Home/Grades  
		public IActionResult Grades()  
        {  
            // Mock grades for demonstration  
            List<EnrollmentModel> grades = new List<EnrollmentModel>  
            {  
				new EnrollmentModel { Id = 1, StudentId = 1, CourseId = 1, CourseName = "ST-MATH", Grade = "3.0" },  
				new EnrollmentModel { Id = 2, StudentId = 1, CourseId = 2, CourseName = "ST-INTSY", Grade = "2.5" }  
			};  
            return View(grades);  
        }  
    }  
}