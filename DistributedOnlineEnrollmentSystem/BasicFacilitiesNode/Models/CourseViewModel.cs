namespace BasicFacilitiesNode.Models
{
	public class CourseViewModel
	{
		public int Id { get; set; }
		public string CourseCode { get; set; }
		public int Units { get; set; }
		public int Capacity { get; set; }
		public int SlotsTaken { get; set; }
		public bool IsEnrolled { get; set; }
		public string Instructor { get; set; }
		public bool isFull { get; set; }
		public List<UserViewModel> Students { get; set; }
	}
}
