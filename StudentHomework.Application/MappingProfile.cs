using AutoMapper;
using StudentHomework.Application.DTOs;
using StudentHomework.Domain.Entities;
using StudentHomework.Domain.Repositories;

namespace StudentHomework.Application
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Student, StudentDto>().ReverseMap();
            CreateMap<Course, CourseDto>()
                .ForMember(dest => dest.CourseDescription, opt => opt.MapFrom(src => src.CourseDescription))
                .ReverseMap();
        }
    }
}
