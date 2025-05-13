namespace StudentHomework.Application.DTOs
{
    public class StudentDto
    {
        public int StudentId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public List<CourseDto>? Courses { get; set; }
    }
}
