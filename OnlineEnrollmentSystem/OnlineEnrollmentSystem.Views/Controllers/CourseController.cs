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

		// GET: /Courses/View
		public async Task<IActionResult> View()
		{
			// Fetch courses from the database
			var courses = await _context.Courses.ToListAsync();

			// Fetch enrollments from the database
			var enrollments = await _context.Enrollments.ToListAsync();

			// Create the view model with courses and enrollments
			var viewModel = new CourseListViewModel
			{
				Courses = courses.Select(course => new CourseViewModel
				{
					Id = course.Id,
					InstructorId = course.InstructorId,
					CourseCode = course.CourseCode,
					Units = course.Units,
					Capacity = course.Capacity,
					// Check if the student is enrolled in this course
					IsEnrolled = enrollments.Any(e => e.CourseId == course.Id)
				}).ToList(),
				Enrollments = enrollments
			};

			return View(viewModel);
		}

		// GET: /Courses/Grades
		public async Task<IActionResult> Grades()
		{
			//var studentIdString = HttpContext.Session.GetString("StudentId");
			//if (string.IsNullOrEmpty(studentIdString) || !int.TryParse(studentIdString, out int studentId))
			//{
			//	return RedirectToAction("Login", "Auth"); // or handle unauthorized access
			//}

			int studentId = 0;

			// Get the grades from the database
			var grades = await _context.Enrollments
									.Where(e => e.StudentId == studentId)
									.ToListAsync(); // returns List<EnrollmentModel>

			return View(grades);
		}
	}
}