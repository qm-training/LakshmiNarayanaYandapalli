namespace WebApiStudentManagement.Infrastructure.Services;

public class TeacherService(ITeacherRepository teacherRepository, IMapper mapper) : ITeacherService
{
    private readonly IMapper _mapper = mapper;
    public string AddTeacher(AddTeacher teacher)
    {
        var newTeacher = _mapper.Map<Teacher>(teacher);
        return teacherRepository.AddTeacher(newTeacher);
    }

    public string DeleteTeacher(string email)
    {
        return teacherRepository.DeleteTeacher(email);
    }

    public List<TeacherResponseDto> GetAllTeachers()
    {
        var teachers = teacherRepository.GetAllTeachers();
        return _mapper.Map<List<TeacherResponseDto>>(teachers);
    }

    public List<CourseResponseDto> GetCoursesOfTeacher(string email)
    {
        var courses = teacherRepository.GetCoursesOfTeacher(email);
        return _mapper.Map<List<CourseResponseDto>>(courses);
    }

    public TeacherResponseDto GetTeacherByEmail(string email)
    {
        var teacher = teacherRepository.GetTeacherByEmail(email);
        return _mapper.Map<TeacherResponseDto>(teacher);
    }

    public TeacherResponseDto UpdateTeacher(AddTeacher teacher, string email)
    {
        var updatedTeacher = _mapper.Map<Teacher>(teacher);
        var result = teacherRepository.UpdateTeacher(updatedTeacher, email);
        return _mapper.Map<TeacherResponseDto>(result);
    }
}
