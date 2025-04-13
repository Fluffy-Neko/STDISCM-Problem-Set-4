using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentNode.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentNode.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "student")]
    public class StudentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StudentController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Student/Grades
        [HttpGet("Grades")]
        public async Task<IActionResult> GetGrades()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized("Invalid or missing user ID");

            var validGrades = new List<string> { "0.0", "1.0", "1.5", "2.0", "2.5", "3.0", "3.5", "4.0" };

            var grades = await _context.Enrollments
                .Where(e => e.StudentId == userId && validGrades.Contains(e.Grade))
                .ToListAsync();

            return Ok(grades);
        }

        // POST: api/Student/Enroll
        [HttpPost("Enroll")]
        public async Task<IActionResult> Enroll([FromBody] EnrollmentRequestModel data)
        {
            if (data == null || data.CourseId <= 0)
                return BadRequest("Invalid request payload.");

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized("Invalid or missing user ID");

            bool alreadyEnrolled = await _context.Enrollments
                .AnyAsync(e => e.StudentId == userId && e.CourseId == data.CourseId);

            if (alreadyEnrolled)
                return BadRequest("You're already enrolled in this course.");

            var course = await _context.Courses.FindAsync(data.CourseId);
            if (course == null)
                return NotFound("Course not found.");

            var courseEnrollments = await _context.Enrollments.Where(e => e.CourseId == course.Id).ToListAsync();
            var courseSlotsTaken = courseEnrollments.Count();
            var courseCapacity = course.Capacity;

            if (courseSlotsTaken >= courseCapacity)
                return BadRequest("Course already full capacity.");

            var enrollment = new EnrollmentModel
            {
                StudentId = userId,
                CourseId = data.CourseId,
                CourseCode = course.CourseCode,
                Grade = "NGA"
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return Ok("Successfully enrolled.");
        }
    }
}
