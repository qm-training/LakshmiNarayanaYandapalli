namespace WebApiStudentManagement.Api.Configuration;
public static class AutoMapperConfiguration
{
    public static IMapper InitializeMapper()
    {
        var mapper = new MapperConfiguration(config =>
        {
            config.CreateMap<CourseVm, Course>()
      .ForMember(dest => dest.Teacher, opt => opt.Ignore());
            config.CreateMap<StudentVm, Student>();
            config.CreateMap<TeacherVm, Teacher>();
            config.CreateMap<Course, CourseResponseDto>();
            config.CreateMap<Student, StudentResponseDto>();
            config.CreateMap<Teacher, TeacherResponseDto>();
        });

        return mapper.CreateMapper();
    }
}
