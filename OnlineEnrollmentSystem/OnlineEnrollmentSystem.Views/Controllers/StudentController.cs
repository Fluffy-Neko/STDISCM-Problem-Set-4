using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineEnrollmentSystem.Data;
using OnlineEnrollmentSystem.Models;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineEnrollmentSystem.Controllers
{
	public class StudentController : Controller
	{
		private readonly AppDbContext _context;

		// Constructor that injects AppDbContext
		public StudentController(AppDbContext context)
		{
			_context = context;
		}

		// GET: /Courses/Grades
		public async Task<IActionResult> Grades()
		{
			var userRole = HttpContext.Session.GetString("Role");
			var userId = HttpContext.Session.GetInt32("UserId");

			if (userId == null)
			{
				return RedirectToAction("Login", "Home");
			}

			if (userRole != "student")
			{
				// MAKE SEPARATE VIEW
				return RedirectToAction("Index", "Home");
			}

			var validGrades = new List<string> { "0.0", "1.0", "1.5", "2.0", "2.5", "3.0", "3.5", "4.0" };

			var grades = await _context.Enrollments
				.Where(e => e.StudentId == userId && validGrades.Contains(e.Grade))
				.ToListAsync();

			return View(grades);
		}

		[HttpPost]
		public async Task<IActionResult> Enroll(int courseId)
		{
			var userRole = HttpContext.Session.GetString("Role");
			var userId = HttpContext.Session.GetInt32("UserId");

			if (userId == null)
			{
				return RedirectToAction("Login", "Home");
			}

			if (userRole != "student")
			{
				// MAKE SEPARATE VIEW
				return RedirectToAction("Index", "Home");
			}

			bool alreadyEnrolled = await _context.Enrollments
				.AnyAsync(e => e.StudentId == userId && e.CourseId == courseId);

			if (alreadyEnrolled)
			{
				TempData["Error"] = "You're already enrolled in this course.";
				return RedirectToAction("Index");
			}

			var course = await _context.Courses.FindAsync(courseId);
			if (course == null)
				return NotFound();

			var enrollment = new EnrollmentModel
			{
				StudentId = userId ?? 0,
				CourseId = courseId,
				CourseCode = course.CourseCode,
				Grade = "NGA"
			};

			_context.Enrollments.Add(enrollment);
			await _context.SaveChangesAsync();

			TempData["Success"] = "Successfully enrolled!";
			return RedirectToAction("Index", "Courses");
		}
	}
}