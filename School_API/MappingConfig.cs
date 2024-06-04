using AutoMapper;
using School_API.Dto;
using SharedModels;

namespace School_API
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Student, StudentDto>().ReverseMap();
            CreateMap<Student, StudentCreateDto>().ReverseMap();
            CreateMap<Student, StudentUpdateDto>().ReverseMap();
        }
    }
}
