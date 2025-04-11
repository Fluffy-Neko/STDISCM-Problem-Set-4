namespace ViewsNode.Models  
{  
    public class EnrollmentModel  
    {  
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public string CourseCode { get; set; }  
        public string Grade { get; set; }  
    }  
}