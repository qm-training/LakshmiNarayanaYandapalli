namespace WebApiStudentManagement.Infrastructure.Services;

public class CourseService(ICourseRepository courseRepository, IMapper mapper) : ICourseService
{
    private readonly ICourseRepository _courseRepository = courseRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<string> AddCourse(AddCourse course)
    {
        var newCourse = _mapper.Map<Course>(course);
        return await _courseRepository.AddCourse(newCourse);
    }

    public async Task<string> DeleteCourse(string title)
    {
        return await _courseRepository.DeleteCourse(title);
    }

    public async Task<List<CourseResponseDto>> GetAllCourses()
    {
        var courses = await _courseRepository.GetAllCourses();
        return _mapper.Map<List<CourseResponseDto>>(courses);
    }

    public async Task<CourseResponseDto> GetCourseByTitle(string title)
    {
        var course = await _courseRepository.GetCourseByTitle(title);
        return _mapper.Map<CourseResponseDto>(course);
    }

    public async Task<List<StudentResponseDto>> GetStudentsInCourse(string title)
    {
        var students = await _courseRepository.GetStudentsInCourse(title);
        return _mapper.Map<List<StudentResponseDto>>(students);
    }

    public async Task<TeacherResponseDto> GetTeacherOfCourse(string title)
    {
        var teacher = await _courseRepository.GetTeacherOfCourse(title);
        return _mapper.Map<TeacherResponseDto>(teacher);
    }

    public async Task<CourseResponseDto> UpdateCourse(AddCourse course, string title)
    {
        var updatedCourse = _mapper.Map<Course>(course);
        var result = await _courseRepository.UpdateCourse(updatedCourse, title);
        return _mapper.Map<CourseResponseDto>(result);
    }
}
