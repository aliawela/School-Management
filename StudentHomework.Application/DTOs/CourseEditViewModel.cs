namespace StudentHomework.Application.DTOs
{
    public class CourseEditViewModel
    {
        public int CourseId { get; set; } // Will be auto-incremented by DB
        public string Title { get; set; } = string.Empty;
        public string CourseDescription { get; set; } = string.Empty;
    }
}
