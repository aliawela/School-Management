using AutoMapper;
using StudentHomework.Application.DTOs;
using StudentHomework.Domain.Entities;
using StudentHomework.Domain.Repositories;

namespace StudentHomework.Application.Managers
{
    public class CourseManager(ICourseRepository courseRepository, IMapper mapper) : BaseManager<Course, CourseDto>(courseRepository, mapper)
    {
        private readonly ICourseRepository _courseRepository = courseRepository;
        // Add custom methods if needed

        public async Task<IEnumerable<CourseDto>> FilterCoursesByNameAsync(string name)
        {
            var courses = await _courseRepository.FilterCoursesByNameAsync(name);
            return _mapper.Map<IEnumerable<CourseDto>>(courses);
        }

        public async Task DeleteCourseAsync(int courseId)
        {
            await _courseRepository.DeleteAsync(courseId);
        }
    }
}
