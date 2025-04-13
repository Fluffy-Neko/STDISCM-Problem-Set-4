using Microsoft.EntityFrameworkCore;

namespace BasicFacilitiesNode.Models
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		public DbSet<CourseModel> Courses { get; set; }
		public DbSet<EnrollmentModel> Enrollments { get; set; }
		public DbSet<UserModel> Users { get; set; }
	}
}