using StudentHomework.Domain.Entities;

namespace StudentHomework.Domain.Repositories
{
    public interface ICourseRepository : IRepository<Course>
    {
        // Add custom methods for Course if needed
        Task<IEnumerable<Course>> GetCoursesByStudentIdAsync(int studentId);
        Task<IEnumerable<Course>> FilterCoursesByNameAsync(string name);
    }
}
