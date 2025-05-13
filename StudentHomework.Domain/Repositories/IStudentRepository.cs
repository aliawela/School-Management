using StudentHomework.Domain.Entities;

namespace StudentHomework.Domain.Repositories
{
    public interface IStudentRepository : IRepository<Student>
    {
        // Add custom methods for Student if needed
        Task<IEnumerable<Student>> GetStudentsByCourseIdAsync(int courseId);
        Task<IEnumerable<Student>> FilterStudentsByNameAsync(string name);
        Task<IEnumerable<Student>> FilterStudentsByEmailAsync(string email);
        Task<Student?> GetByIdWithCoursesAsync(int id);
    }
}
