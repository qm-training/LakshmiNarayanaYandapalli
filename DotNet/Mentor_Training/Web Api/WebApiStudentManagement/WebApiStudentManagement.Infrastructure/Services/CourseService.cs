namespace WebApiStudentManagement.Infrastructure.Services;

public class CourseService(ICourseRepository courseRepository, IMapper mapper) : ICourseService
{
    private readonly IMapper _mapper = mapper;
    public string AddCourse(AddCourse course)
    {
        var newCourse = _mapper.Map<Course>(course);
        return courseRepository.AddCourse(newCourse);
    }

    public string DeleteCourse(string email)
    {
        return courseRepository.DeleteCourse(email);
    }

    public List<CourseResponseDto> GetAllCourses()
    {
        var courses = courseRepository.GetAllCourses();
        return _mapper.Map<List<CourseResponseDto>>(courses);
    }

    public CourseResponseDto GetCourseByTitle(string title)
    {
        var course = courseRepository.GetCourseByTitle(title);
        return _mapper.Map<CourseResponseDto>(course);
    }

    public List<StudentResponseDto> GetStudentsInCourse(string title)
    {
        var students = courseRepository.GetStudentsInCourse(title);
        return _mapper.Map<List<StudentResponseDto>>(students);
    }

    public TeacherResponseDto GetTeacherOfCourse(string title)
    {
        var teacher = courseRepository.GetTeacherOfCourse(title);
        return _mapper.Map<TeacherResponseDto>(teacher);
    }

    public CourseResponseDto UpdateCourse(AddCourse course, string title)
    {
        var updatedCourse = _mapper.Map<Course>(course);
        var result = courseRepository.UpdateCourse(updatedCourse, title);
        return _mapper.Map<CourseResponseDto>(result);
    }
}
