namespace WebApiStudentManagement.Infrastructure.Services;

public class StudentService(IStudentRepository studentRepository, IMapper mapper) : IStudentService
{
    private readonly IStudentRepository _studentRepository = studentRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<string> AddStudentAsync(StudentVm student)
    {
        var newStudent = _mapper.Map<Student>(student);
        return await _studentRepository.AddStudentAsync(newStudent);
    }

    public async Task<string> DeleteStudentAsync(string email)
    {
        return await _studentRepository.DeleteStudentAsync(email);
    }

    public async Task<string> EnrollStudentInCourseAsync(string studentEmail, string courseTitle)
    {
        return await _studentRepository.EnrollStudentInCourseAsync(studentEmail, courseTitle);
    }

    public async Task<List<StudentResponseDto>> GetAllStudentsAsync()
    {
        var students = await _studentRepository.GetAllStudentsAsync();
        return _mapper.Map<List<StudentResponseDto>>(students);
    }

    public async Task<List<CourseResponseDto>> GetCoursesOfStudentAsync(string email)
    {
        var courses = await _studentRepository.GetCoursesOfStudentAsync(email);
        return _mapper.Map<List<CourseResponseDto>>(courses);
    }

    public async Task<StudentResponseDto> GetStudentByEmailAsync(string email)
    {
        var student = await _studentRepository.GetStudentByEmailAsync(email);
        return student == null ? null : _mapper.Map<StudentResponseDto>(student);
    }

    public async Task<StudentResponseDto> UpdateStudentAsync(StudentVm student, string email)
    {
        var updatedStudent = _mapper.Map<Student>(student);
        var result = await _studentRepository.UpdateStudentAsync(updatedStudent, email);
        return _mapper.Map<StudentResponseDto>(result);
    }
}
