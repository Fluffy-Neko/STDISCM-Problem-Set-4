using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineEnrollmentSystem.Data;
using OnlineEnrollmentSystem.Models;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineEnrollmentSystem.Controllers
{
	public class InstructorController : Controller
	{
		private readonly AppDbContext _context;

		// Constructor that injects AppDbContext
		public InstructorController(AppDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<IActionResult> View(int id)
		{
			// Retrieve the logged-in user's ID and role from the session
			var userRole = HttpContext.Session.GetString("Role");
			var userId = HttpContext.Session.GetInt32("UserId");

			if (userId == null)
			{
				return RedirectToAction("Login", "Home");
			}

			if (userRole != "instructor")
			{
				// MAKE SEPARATE VIEW
				return RedirectToAction("Index", "Home");
			}

			var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);

			if (course == null)
			{
				return NotFound();
			}

			var instructor = await _context.Users.FirstOrDefaultAsync(u => u.Id == course.InstructorId);

			var enrollments = await _context.Enrollments
				.Where(e => e.CourseId == id)
				.ToListAsync();

			var studentIds = enrollments.Select(e => e.StudentId).ToList();
			var students = await _context.Users
				.Where(u => studentIds.Contains(u.Id))
				.ToListAsync();

			var studentViewModels = students.Select(s => new UserViewModel
			{
				Id = s.Id,
				Username = s.Username
			}).ToList();

			ViewBag.StudentData = students.Select(s => new
			{
				Name = s.Username,
				StudentId = s.Id,
				Grade = enrollments.FirstOrDefault(e => e.StudentId == s.Id)?.Grade ?? "N/A"
			}).ToList();

			var viewModel = new CourseViewModel
			{
				Id = course.Id,
				CourseCode = course.CourseCode,
				Units = course.Units,
				Capacity = course.Capacity,
				SlotsTaken = enrollments.Count,
				Instructor = instructor?.Username ?? "Unknown",
				Students = studentViewModels
			};

			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> UpdateGrade(int studentId, int courseId, string grade)
		{
			var userRole = HttpContext.Session.GetString("Role");
			var userId = HttpContext.Session.GetInt32("UserId");

			if (userId == null)
			{
				return RedirectToAction("Login", "Home");
			}

			if (userRole != "instructor")
			{
				// MAKE SEPARATE VIEW
				return RedirectToAction("Index", "Home");
			}

			var validGrades = new List<string> { "NGA", "0.0", "1.0", "1.5", "2.0", "2.5", "3.0", "3.5", "4.0" };

			if (!validGrades.Contains(grade))
			{
				ModelState.AddModelError(string.Empty, "Invalid grade value.");
				return RedirectToAction("View", new { id = courseId });
			}

			var enrollment = await _context.Enrollments
				.FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

			if (enrollment == null)
			{
				return NotFound("Enrollment not found.");
			}

			enrollment.Grade = grade;

			await _context.SaveChangesAsync();

			return RedirectToAction("View", new { id = courseId });
		}
	}
}