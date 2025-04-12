using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InstructorNode.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InstructorNode.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "instructor")]
    public class InstructorController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InstructorController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("View")]
        public async Task<IActionResult> GetCourseDetails(int id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized("Invalid or missing user ID");

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id && c.InstructorId == userId);
            if (course == null)
                return NotFound("Course not found or you are not the instructor.");

            var instructor = await _context.Users.FirstOrDefaultAsync(u => u.Id == course.InstructorId);
            var enrollments = await _context.Enrollments.Where(e => e.CourseId == id).ToListAsync();

            var studentIds = enrollments.Select(e => e.StudentId).ToList();
            var students = await _context.Users.Where(u => studentIds.Contains(u.Id)).ToListAsync();

            var studentViewModels = students.Select(s => new UserViewModel
            {
                Id = s.Id,
                Username = s.Username,
                Grade = enrollments.FirstOrDefault(e => e.StudentId == s.Id)?.Grade ?? "NGA"
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

            return Ok(viewModel);
        }

        [HttpPost("UpdateGrade")]
        public async Task<IActionResult> UpdateGrade([FromBody] GradeUpdateRequest model)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized("Invalid or missing user ID");

            var userRole = User.FindFirstValue(ClaimTypes.Role);
            if (userRole != "instructor")
                return Forbid("Only instructors can update grades.");

            var validGrades = new List<string> { "NGA", "0.0", "1.0", "1.5", "2.0", "2.5", "3.0", "3.5", "4.0" };
            if (!validGrades.Contains(model.Grade))
                return BadRequest("Invalid grade value.");

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == model.CourseId && c.InstructorId == userId);
            if (course == null)
                return NotFound("Course not found or you're not the instructor.");

            var enrollment = await _context.Enrollments.FirstOrDefaultAsync(e =>
                e.StudentId == model.StudentId && e.CourseId == model.CourseId);

            if (enrollment == null)
                return NotFound("Enrollment not found.");

            enrollment.Grade = model.Grade;
            await _context.SaveChangesAsync();

            return Ok("Grade updated successfully.");
        }
    }

    public class GradeUpdateRequest
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public string Grade { get; set; }
    }
}
