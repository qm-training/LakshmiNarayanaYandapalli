using AutoMapper;

namespace LibraryManagementRedis.Api.Configuration;
public static class AutoMapperConfiguration
{
    public static IMapper InitializeMapper()
    {
        var mapper = new MapperConfiguration(config =>
        {
            config.CreateMap<CourseVm, Course>();
            config.CreateMap<StudentVm, Student>();
            config.CreateMap<TeacherVm, Teacher>();
            config.CreateMap<CourseResponseDto, Course>();
            config.CreateMap<StudentResponseDto, Student>();
            config.CreateMap<TeacherResponseDto, Teacher>();
        });

        return mapper.CreateMapper();
    }
}
