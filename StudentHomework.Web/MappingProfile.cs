using AutoMapper;
using StudentHomework.Application.DTOs;
using StudentHomework.Web.Models;

namespace StudentHomework.Web
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CourseEditViewModel, CourseDto>()
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.CourseDescription, opt => opt.MapFrom(src => src.CourseDescription));
        }
    }
}
