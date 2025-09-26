using WebApiStudentManagement.Core.Vms;

namespace WebApiStudentManagement.Infrastructure.Services;

public class CourseService(ICourseRepository courseRepository, IMapper mapper) : ICourseService
{
    private readonly ICourseRepository _courseRepository = courseRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<string> AddCourseAsync(CourseVm course)
    {
        var newCourse = _mapper.Map<Course>(course);
        return await _courseRepository.AddCourseAsync(newCourse);
    }

    public async Task<string> DeleteCourseAsync(string title)
    {
        return await _courseRepository.DeleteCourseAsync(title);
    }

    public async Task<List<CourseResponseDto>> GetAllCoursesAsync()
    {
        var courses = await _courseRepository.GetAllCoursesAsync();
        return _mapper.Map<List<CourseResponseDto>>(courses);
    }

    public async Task<CourseResponseDto> GetCourseByTitleAsync(string title)
    {
        var course = await _courseRepository.GetCourseByTitleAsync(title);
        return _mapper.Map<CourseResponseDto>(course);
    }

    public async Task<List<StudentResponseDto>> GetStudentsInCourseAsync(string title)
    {
        var students = await _courseRepository.GetStudentsInCourseAsync(title);
        return _mapper.Map<List<StudentResponseDto>>(students);
    }

    public async Task<TeacherResponseDto> GetTeacherOfCourseAsync(string title)
    {
        var teacher = await _courseRepository.GetTeacherOfCourseAsync(title);
        return _mapper.Map<TeacherResponseDto>(teacher);
    }

    public async Task<CourseResponseDto> UpdateCourseAsync(CourseVm course, string title)
    {
        var updatedCourse = _mapper.Map<Course>(course);
        var result = await _courseRepository.UpdateCourseAsync(updatedCourse, title);
        return _mapper.Map<CourseResponseDto>(result);
    }
}
