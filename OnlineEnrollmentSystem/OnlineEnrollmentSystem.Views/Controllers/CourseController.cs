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
			int studentId = 0;

			// Fetch courses from the database
			var courses = await _context.Courses.ToListAsync();

			// Fetch enrollments from the database
			var enrollments = await _context.Enrollments
										.Where(e => e.StudentId == studentId)
										.ToListAsync();

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

			int studentId = 5;

			var validGrades = new List<string> { "0.0", "1.0", "1.5", "2.0", "2.5", "3.0", "3.5", "4.0" };

			var grades = await _context.Enrollments
				.Where(e => e.StudentId == studentId && validGrades.Contains(e.Grade))
				.ToListAsync();

			return View(grades);
		}


		[HttpPost]
		public async Task<IActionResult> Enroll(int courseId)
		{
			// Example: get logged-in student ID from session or claims
			//var studentId = HttpContext.Session.GetInt32("StudentId"); // or however you're storing it
			//if (studentId == null)
			//	return RedirectToAction("Login", "Auth");

			int studentId = 0;

			// Check if already enrolled
			bool alreadyEnrolled = await _context.Enrollments
				.AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);

			if (alreadyEnrolled)
			{
				TempData["Error"] = "You're already enrolled in this course.";
				return RedirectToAction("View");
			}

			// Enroll the student
			var course = await _context.Courses.FindAsync(courseId);
			if (course == null)
				return NotFound();

			var enrollment = new EnrollmentModel
			{
				StudentId = studentId,
				CourseId = courseId,
				CourseCode = course.CourseCode,
				Grade = "NGA"
			};

			_context.Enrollments.Add(enrollment);
			await _context.SaveChangesAsync();

			TempData["Success"] = "Successfully enrolled!";
			return RedirectToAction("View");
		}
	}
}