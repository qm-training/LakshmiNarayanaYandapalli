namespace WebApiStudentManagement.Infrastructure.Services;

public class StudentService(IStudentRepository studentRepository, IMapper mapper) : IStudentService
{
    private readonly IStudentRepository _studentRepository = studentRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<string> AddStudent(AddStudent student)
    {
        var newStudent = _mapper.Map<Student>(student);
        return await _studentRepository.AddStudent(newStudent);
    }

    public async Task<string> DeleteStudent(string email)
    {
        return await _studentRepository.DeleteStudent(email);
    }

    public async Task<string> EnrollStudentInCourse(string studentEmail, string courseTitle)
    {
        return await _studentRepository.EnrollStudentInCourse(studentEmail, courseTitle);
    }

    public async Task<List<StudentResponseDto>> GetAllStudents()
    {
        var students = await _studentRepository.GetAllStudents();
        return _mapper.Map<List<StudentResponseDto>>(students);
    }

    public async Task<List<CourseResponseDto>> GetCoursesOfStudent(string email)
    {
        var courses = await _studentRepository.GetCoursesOfStudent(email);
        return _mapper.Map<List<CourseResponseDto>>(courses);
    }

    public async Task<StudentResponseDto> GetStudentByEmail(string email)
    {
        var student = await _studentRepository.GetStudentByEmail(email);
        return student == null ? null : _mapper.Map<StudentResponseDto>(student);
    }

    public async Task<StudentResponseDto> UpdateStudent(AddStudent student, string email)
    {
        var updatedStudent = _mapper.Map<Student>(student);
        var result = await _studentRepository.UpdateStudent(updatedStudent, email);
        return _mapper.Map<StudentResponseDto>(result);
    }
}
