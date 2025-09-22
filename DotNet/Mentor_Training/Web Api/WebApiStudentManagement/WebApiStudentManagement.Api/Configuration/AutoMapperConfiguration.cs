namespace WebApiStudentManagement.Api.Configuration
{
    public static class AutoMapperConfiguration
    {
        public static IMapper InitializeMapper()
        {
            var mapper = new MapperConfiguration(config =>
            {
                config.CreateMap<AddCourse, Course>();
                config.CreateMap<AddStudent, Student>();
                config.CreateMap<AddTeacher, Teacher>();
                config.CreateMap<CourseResponseDto, Course>();
                config.CreateMap<StudentResponseDto, Student>();
                config.CreateMap<TeacherResponseDto, Teacher>();
            });

            return mapper.CreateMapper();
        }
    }
}
