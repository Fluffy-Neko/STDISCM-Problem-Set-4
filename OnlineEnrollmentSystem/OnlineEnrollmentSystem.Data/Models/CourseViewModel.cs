namespace OnlineEnrollmentSystem.Models
{
	public class CourseViewModel
	{
		public int Id { get; set; }
		public int InstructorId { get; set; }
		public string CourseCode { get; set; }
		public int Units { get; set; }
		public int Capacity { get; set; }
		public bool IsEnrolled { get; set; }
	}
}
