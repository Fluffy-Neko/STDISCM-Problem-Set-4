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
			return RedirectToAction("Index");
		}
	}
}