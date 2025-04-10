using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector.Logging;
using OnlineEnrollmentSystem.Data;
using OnlineEnrollmentSystem.Models;
using Org.BouncyCastle.Security;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineEnrollmentSystem.Controllers  
{  
    public class HomeController : Controller  
    {
		private readonly AppDbContext _context;

		public HomeController(AppDbContext context)
		{
			_context = context;
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
				TempData["id"] = user.Id;
				TempData["role"] = user.Role;

				return RedirectToAction("Index", "Courses");
			}

			ModelState.AddModelError(string.Empty, "Invalid credentials.");
			return View(loginRequest);
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