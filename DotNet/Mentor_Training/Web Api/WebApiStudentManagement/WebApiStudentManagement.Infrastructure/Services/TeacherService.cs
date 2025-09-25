namespace WebApiStudentManagement.Infrastructure.Services;

public class TeacherService(ITeacherRepository teacherRepository, IMapper mapper) : ITeacherService
{
    private readonly ITeacherRepository _teacherRepository = teacherRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<string> AddTeacher(AddTeacher teacher)
    {
        var newTeacher = _mapper.Map<Teacher>(teacher);
        return await _teacherRepository.AddTeacher(newTeacher);
    }

    public async Task<string> DeleteTeacher(string email)
    {
        return await _teacherRepository.DeleteTeacher(email);
    }

    public async Task<List<TeacherResponseDto>> GetAllTeachers()
    {
        var teachers = await _teacherRepository.GetAllTeachers();
        return _mapper.Map<List<TeacherResponseDto>>(teachers);
    }

    public async Task<List<CourseResponseDto>> GetCoursesOfTeacher(string email)
    {
        var courses = await _teacherRepository.GetCoursesOfTeacher(email);
        return _mapper.Map<List<CourseResponseDto>>(courses);
    }

    public async Task<TeacherResponseDto> GetTeacherByEmail(string email)
    {
        var teacher = await _teacherRepository.GetTeacherByEmail(email);
        return teacher == null ? null : _mapper.Map<TeacherResponseDto>(teacher);
    }

    public async Task<TeacherResponseDto> UpdateTeacher(AddTeacher teacher, string email)
    {
        var updatedTeacher = _mapper.Map<Teacher>(teacher);
        var result = await _teacherRepository.UpdateTeacher(updatedTeacher, email);
        return _mapper.Map<TeacherResponseDto>(result);
    }
}
