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
			var viewModel = new CourseListViewModel();
			bool userRole = false;

			if (!userRole)
			{
				int studentId = 0;

				var courses = await _context.Courses.ToListAsync();

				var enrollments = await _context.Enrollments
					.Where(e => e.StudentId == studentId)
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
			else if (userRole)
			{
				//int instructorId = 1;

				//var courses = await _context.Courses
				//	.Where(c => c.InstructorId == instructorId)
				//	.ToListAsync();

				//var courseIds = courses.Select(c => c.Id).ToList();

				//var enrollments = await _context.Enrollments
				//	.Where(e => courseIds.Contains(e.CourseId))
				//	.ToListAsync();

				//var students = await _context.Users
				//	.Where(u => enrollments.Select(e => e.StudentId).Contains(u.Id))
				//	.ToListAsync();

				//var instructors = await _context.Users
				//	.Where(u => courses.Select(c => c.InstructorId).Contains(u.Id))
				//	.ToListAsync();

				//viewModel.Courses = courses.Select(course =>
				//{
				//	var courseEnrollments = enrollments.Where(e => e.CourseId == course.Id).ToList();
				//	var courseStudents = students.Where(s => courseEnrollments.Select(e => e.StudentId).Contains(s.Id)).ToList();
				//	var instructor = instructors.FirstOrDefault(i => i.Id == course.InstructorId);

				//	return new CourseViewModel
				//	{
				//		Id = course.Id,
				//		InstructorId = course.InstructorId,
				//		CourseCode = course.CourseCode,
				//		Units = course.Units,
				//		Capacity = course.Capacity,
				//		SlotsTaken = courseEnrollments.Count,
				//		IsEnrolled = true,
				//		Instructor = instructor?.Username ?? "Unknown",
				//		Students = courseStudents
				//	};
				//}).ToList();
			}
			else
			{
				return NotFound();
			}

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