using StudentHomework.Domain.Entities;
using StudentHomework.Domain.Repositories;
using StudentHomework.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace StudentHomework.Infrastructure.Repositories
{
    public class StudentRepository(StudentHomeworkDbContext context) : Repository<Student>(context), IStudentRepository
    {
        public async Task<IEnumerable<Student>> GetStudentsByCourseIdAsync(int courseId)
        {
            return await _dbSet.Where(s => s.Courses.Any(c => c.CourseId == courseId)).ToListAsync();
        }

        public async Task<IEnumerable<Student>> FilterStudentsByNameAsync(string name)
        {
            return await _dbSet.Where(s => s.Name != null && s.Name.ToLower().Contains(name.ToLower())).ToListAsync();
        }

        public async Task<IEnumerable<Student>> FilterStudentsByEmailAsync(string email)
        {
            return await _dbSet.Where(s => s.Email != null && s.Email.ToLower().Contains(email.ToLower())).ToListAsync();
        }

        public async Task<Student?> GetByIdWithCoursesAsync(int id)
        {
            return await _dbSet.Include(s => s.Courses).FirstOrDefaultAsync(s => s.StudentId == id);
        }
    }
}
