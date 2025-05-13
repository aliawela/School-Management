using System.ComponentModel.DataAnnotations;

namespace StudentHomework.Domain.Entities
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Course name is required.")]
        [StringLength(100, ErrorMessage = "Course name cannot be longer than 100 characters.")]
        public required string CourseName { get; set; }

        [Required(ErrorMessage = "Course description is required.")]
        [StringLength(500, ErrorMessage = "Course description cannot be longer than 500 characters.")]
        public required string CourseDescription { get; set; }

        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
