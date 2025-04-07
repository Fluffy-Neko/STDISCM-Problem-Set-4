using Microsoft.EntityFrameworkCore;
using OnlineEnrollmentSystem.Models;  // Make sure to import the correct namespace

namespace OnlineEnrollmentSystem.Data
{
    public class AppDbContext : DbContext
    {
        // Define DbSets for your models (representing tables in the database)
        public DbSet<CourseModel> Courses { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<EnrollmentModel> Enrollments { get; set; }

        // Constructor to pass DbContextOptions to the base class
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Optionally, you can override the OnModelCreating method to configure your database schema (relationships, constraints, etc.)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Example of configuring relationships
            // modelBuilder.Entity<EnrollmentModel>()
            //     .HasOne(e => e.StudentId)
            //     .WithMany(u => u.Enrollments)
            //     .HasForeignKey(e => e.UserId);

            // modelBuilder.Entity<EnrollmentModel>()
            //     .HasOne(e => e.Course)
            //     .WithMany(c => c.Enrollments)
            //     .HasForeignKey(e => e.CourseId);
        }
    }
}
