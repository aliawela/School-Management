using AutoMapper;
using StudentHomework.Application.DTOs;
using StudentHomework.Domain.Entities;
using StudentHomework.Domain.Repositories;

namespace StudentHomework.Application.Managers
{
    public class StudentManager(IStudentRepository studentRepository, IMapper mapper) : BaseManager<Student, StudentDto>(studentRepository, mapper)
    {
        private readonly IStudentRepository _studentRepository = studentRepository;
        // Add custom methods if needed

        public async Task<IEnumerable<StudentDto>> FilterStudentsByNameAsync(string name)
        {
            var students = await _studentRepository.FilterStudentsByNameAsync(name?.ToLowerInvariant() ?? string.Empty);
            return _mapper.Map<IEnumerable<StudentDto>>(students);
        }

        public async Task<IEnumerable<StudentDto>> FilterStudentsByEmailAsync(string email)
        {
            var students = await _studentRepository.FilterStudentsByEmailAsync(email?.ToLowerInvariant() ?? string.Empty);
            return _mapper.Map<IEnumerable<StudentDto>>(students);
        }

        public async Task DeleteStudentAsync(int studentId)
        {
            await _studentRepository.DeleteAsync(studentId);
        }

        public async Task DropCourseAsync(int studentId, int courseId)
        {
            var student = await _studentRepository.GetByIdWithCoursesAsync(studentId);
            if (student == null || student.Courses == null) return;
            var course = student.Courses.FirstOrDefault(c => c.CourseId == courseId);
            if (course != null)
            {
                student.Courses.Remove(course);
                await _studentRepository.UpdateAsync(student);
            }
        }
    }
}
