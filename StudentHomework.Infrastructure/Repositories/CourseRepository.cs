using StudentHomework.Domain.Entities;
using StudentHomework.Domain.Repositories;
using StudentHomework.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace StudentHomework.Infrastructure.Repositories
{
    public class CourseRepository(StudentHomeworkDbContext context) : Repository<Course>(context), ICourseRepository
    {

        public async Task<IEnumerable<Course>> GetCoursesByStudentIdAsync(int studentId)
        {
            return await _dbSet.Where(c => c.Students.Any(s => s.StudentId == studentId)).ToListAsync();
        }

        public async Task<IEnumerable<Course>> FilterCoursesByNameAsync(string name)
        {
            return await _dbSet.Where(c => c.CourseName != null && c.CourseName.Contains(name)).ToListAsync();
        }

        public override async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
    }
}
