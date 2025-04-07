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
				return RedirectToAction("Index", "Courses");
			}  

            ModelState.AddModelError(string.Empty, "Invalid credentials.");  
            return View();  
        }

		// GET: /Home/Courses  
		//public IActionResult Courses()
		//{
		//	var courses = new List<CourseModel>
		//       {
		//        new CourseModel { Id = 1, InstructorId = 1, CourseCode = "ST-MATH", Units = 3, Capacity = 40 },
		//        new CourseModel { Id = 2, InstructorId = 2, CourseCode = "ST-INTSY", Units = 3, Capacity = 40 },
		//		new CourseModel { Id = 3, InstructorId = 3, CourseCode = "CS-OPESY", Units = 3, Capacity = 40 }
		//	};

		//	var enrollments = new List<EnrollmentModel>
		//       {
		//        new EnrollmentModel { Id = 1, StudentId = 1, CourseId = 1, CourseName = "ST-MATH", Grade = "3.0" }
		//       };

		//	var viewModel = new CourseListViewModel
		//	{
		//	    Courses = courses,
		//	    Enrollments = enrollments
		//	};

		//	return View(viewModel);
		//}


		//// GET: /Home/Grades  
		//public IActionResult Grades()  
  //      {  
  //          // Mock grades for demonstration  
  //          List<EnrollmentModel> grades = new List<EnrollmentModel>  
  //          {
		//		new EnrollmentModel { Id = 1, StudentId = 1, CourseId = 1, CourseCode = "ST-MATH", Grade = "3.0" },
		//		new EnrollmentModel { Id = 2, StudentId = 1, CourseId = 2, CourseCode = "ST-INTSY", Grade = "2.5" }
		//	};  
  //          return View(grades);  
  //      }
    }  
}