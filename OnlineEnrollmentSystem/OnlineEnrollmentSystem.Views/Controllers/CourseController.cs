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

		// GET: /Courses/Index
		public async Task<IActionResult> Index()
		{
			var viewModel = new CourseListViewModel();
			var userRole = HttpContext.Session.GetString("Role");
			var userId = HttpContext.Session.GetInt32("UserId");

			if (userId == null)
			{
				return RedirectToAction("Login", "Home");
			}

			if (userRole == "student")
			{
				var courses = await _context.Courses.ToListAsync();

				var enrollments = await _context.Enrollments
					.Where(e => e.StudentId == userId)
					.ToListAsync();

				var instructorIds = courses.Select(c => c.InstructorId).Distinct().ToList();

				var instructors = await _context.Users
					.Where(u => instructorIds.Contains(u.Id))
					.ToListAsync();

				viewModel.Courses = courses.Select(course =>
				{
					var courseEnrollments = enrollments.Where(e => e.CourseId == course.Id).ToList();
					var instructor = instructors.FirstOrDefault(i => i.Id == course.InstructorId);

					return new CourseViewModel
					{
						Id = course.Id,
						CourseCode = course.CourseCode,
						Units = course.Units,
						Capacity = course.Capacity,
						SlotsTaken = courseEnrollments.Count,
						IsEnrolled = enrollments.Any(e => e.CourseId == course.Id),
						Instructor = instructor?.Username ?? "Unknown",
					};
				}).ToList();
			}
			else if (userRole == "instructor")
			{
				var courses = await _context.Courses
					.Where(c => c.InstructorId == userId)
					.ToListAsync();

				var courseIds = courses.Select(c => c.Id).ToList();

				var enrollments = await _context.Enrollments
					.Where(e => courseIds.Contains(e.CourseId))
					.ToListAsync();

				string instructor = await _context.Users
					.Where(u => u.Id == userId)
					.Select(u => u.Username)
					.FirstOrDefaultAsync();

				var students = await _context.Users
					.Where(u => enrollments.Select(e => e.StudentId).Contains(u.Id))
					.ToListAsync();

				viewModel.Courses = courses.Select(course =>
				{
					var courseEnrollments = enrollments
						.Where(e => e.CourseId == course.Id)
						.ToList();

					var courseStudents = students
						.Where(s => courseEnrollments.Select(e => e.StudentId).Contains(s.Id))
						.Select(s => new UserViewModel
						{
							Id = s.Id,
							Username = s.Username
						})
						.ToList();

					return new CourseViewModel
					{
						Id = course.Id,
						CourseCode = course.CourseCode,
						Units = course.Units,
						Capacity = course.Capacity,
						SlotsTaken = courseEnrollments.Count,
						IsEnrolled = true,
						Instructor = instructor,
						Students = courseStudents
					};
				}).ToList();
			}
			else
			{
				return NotFound();
			}

			ViewBag.UserRole = userRole;
			return View(viewModel);
		}
	}
}