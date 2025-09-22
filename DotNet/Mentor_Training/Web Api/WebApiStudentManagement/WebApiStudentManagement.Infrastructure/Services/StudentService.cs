namespace WebApiStudentManagement.Infrastructure.Services;

public class StudentService(IStudentRepository studentRepository, IMapper mapper) : IStudentService
{
    private readonly IMapper _mapper = mapper;
    public string AddStudent(AddStudent student)
    {
        var newStudent = _mapper.Map<Student>(student);
        return studentRepository.AddStudent(newStudent);
    }

    public string DeleteStudent(string email)
    {
        return studentRepository.DeleteStudent(email);
    }

    public string EnrollStudentInCourse(string studentEmail, string courseTitle)
    {
        return studentRepository.EnrollStudentInCourse(studentEmail, courseTitle);
    }

    public List<StudentResponseDto> GetAllStudents()
    {
        var students = studentRepository.GetAllStudents();
        return _mapper.Map<List<StudentResponseDto>>(students);
    }

    public List<CourseResponseDto> GetCoursesOfStudent(string email)
    {
        var courses = studentRepository.GetCoursesOfStudent(email);
        return _mapper.Map<List<CourseResponseDto>>(courses);
    }

    public StudentResponseDto GetStudentByEmail(string email)
    {
        var student = studentRepository.GetStudentByEmail(email);
        return student == null ? null : _mapper.Map<StudentResponseDto>(student);
    }

    public StudentResponseDto UpdateStudent(AddStudent student, string email)
    {
        var updatedStudent = _mapper.Map<Student>(student);
        var result = studentRepository.UpdateStudent(updatedStudent, email);
        return _mapper.Map<StudentResponseDto>(result);
    }
}
