using WebApiStudentManagement.Core.Vms;

namespace WebApiStudentManagement.Infrastructure.Services;

public class TeacherService(ITeacherRepository teacherRepository, IMapper mapper) : ITeacherService
{
    private readonly ITeacherRepository _teacherRepository = teacherRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<string> AddTeacherAsync(TeacherVm teacher)
    {
        var newTeacher = _mapper.Map<Teacher>(teacher);
        return await _teacherRepository.AddTeacherAsync(newTeacher);
    }

    public async Task<string> DeleteTeacherAsync(string email)
    {
        return await _teacherRepository.DeleteTeacherAsync(email);
    }

    public async Task<List<TeacherResponseDto>> GetAllTeachersAsync()
    {
        var teachers = await _teacherRepository.GetAllTeachersAsync();
        return _mapper.Map<List<TeacherResponseDto>>(teachers);
    }

    public async Task<List<CourseResponseDto>> GetCoursesOfTeacherAsync(string email)
    {
        var courses = await _teacherRepository.GetCoursesOfTeacherAsync(email);
        return _mapper.Map<List<CourseResponseDto>>(courses);
    }

    public async Task<TeacherResponseDto> GetTeacherByEmailAsync(string email)
    {
        var teacher = await _teacherRepository.GetTeacherByEmailAsync(email);
        return teacher == null ? null : _mapper.Map<TeacherResponseDto>(teacher);
    }

    public async Task<TeacherResponseDto> UpdateTeacherAsync(TeacherVm teacher, string email)
    {
        var updatedTeacher = _mapper.Map<Teacher>(teacher);
        var result = await _teacherRepository.UpdateTeacherAsync(updatedTeacher, email);
        return _mapper.Map<TeacherResponseDto>(result);
    }
}
