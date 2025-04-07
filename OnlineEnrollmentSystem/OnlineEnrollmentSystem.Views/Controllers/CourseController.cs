using Microsoft.AspNetCore.Mvc;  
using Microsoft.EntityFrameworkCore;  
using OnlineEnrollmentSystem.Data;  
using OnlineEnrollmentSystem.Models;  
using System.Linq;  
using System.Threading.Tasks;  

namespace OnlineEnrollmentSystem.Controllers  
{  
    public class CoursesController : Controller  
    {  
        private readonly AppDbContext _context;  

        // Constructor that injects AppDbContext  
        public CoursesController(AppDbContext context)  
        {  
            _context = context;  
        }  

        // GET: /Courses  
        public async Task<IActionResult> View()  
        {  
            // Fetch courses from the database  
            var courses = await _context.Courses.ToListAsync();  

            // Fetch enrollments from the database  
            var enrollments = await _context.Enrollments.ToListAsync();  

            // Create the view model with courses and enrollments  
            var viewModel = new CourseListViewModel  
            {  
                Courses = courses.Select(course => new CourseModel  
                {  
                    Id = course.Id,  
                    InstructorId = course.InstructorId,  
                    CourseCode = course.CourseCode,  
                    Units = course.Units,  
                    Capacity = course.Capacity,  
                }).ToList(),  
                Enrollments = enrollments  
            };  

            return View(viewModel);  
        }  

        // GET: /Courses/Enroll  
        public async Task<IActionResult> Enroll()  
        {  
            // Fetch courses from the database  
            var courses = await _context.Courses.ToListAsync();  

            // Fetch enrollments from the database  
            var enrollments = await _context.Enrollments.ToListAsync();  

            // Create the view model with courses and enrollments  
            var viewModel = new CourseListViewModel  
            {  
                Courses = courses.Select(course => new CourseModel  
                {  
                    Id = course.Id,  
                    InstructorId = course.InstructorId,  
                    CourseCode = course.CourseCode,  
                    Units = course.Units,  
                    Capacity = course.Capacity,  
                }).ToList(),  
                Enrollments = enrollments  
            };  

            return View(viewModel);  
        }  

        // GET: /Courses/Faculty  
		public async Task<IActionResult> Faculty()  
		{  
			// Fetch any required data from the database (example structure)  
			var courses = await _context.Courses.ToListAsync();  
			var enrollments = await _context.Enrollments.ToListAsync();  
			
			// Create a ViewModel (currently no specific view model, so pass dummy/empty data for now)  
			var viewModel = new CourseListViewModel  
			{  
				Courses = courses.Select(course => new CourseModel  
				{  
					Id = course.Id,  
					InstructorId = course.InstructorId,  
					CourseCode = course.CourseCode,  
					Units = course.Units,  
					Capacity = course.Capacity,  
				}).ToList(),  
				Enrollments = enrollments  
			};  

			// Return the Faculty.cshtml with the view model  
			return View(viewModel);  
		} 
    }  
}