namespace WebApiStudentManagement.Infrastructure.Services;

public class TeacherService(ITeacherRepository teacherRepository) : ITeacherService
{
    public string AddTeacher(AddTeacher teacher)
    {
        var newTeacher = new Teacher
        {
            FirstName = teacher.FirstName,
            LastName = teacher.LastName,
            Email = teacher.Email
        };
        return teacherRepository.AddTeacher(newTeacher);
    }

    public string DeleteTeacher(string email)
    {
        return teacherRepository.DeleteTeacher(email);
    }

    public List<TeacherResponseDto> GetAllTeachers()
    {
        var teachers = teacherRepository.GetAllTeachers();
        var teacherDtos = teachers.Select(t => new TeacherResponseDto
        {
            FirstName = t.FirstName,
            LastName = t.LastName,
            Email = t.Email
        }).ToList();
        return teacherDtos;
    }

    public List<CourseResponseDto> GetCoursesOfTeacher(string email)
    {
        var courses = teacherRepository.GetCoursesOfTeacher(email);
        var courseDtos = courses.Select(c => new CourseResponseDto
        {
            Title = c.Title
        }).ToList();
        return courseDtos;
    }

    public TeacherResponseDto GetTeacherByEmail(string email)
    {
        var teacher = teacherRepository.GetTeacherByEmail(email);
        var teacherDto = new TeacherResponseDto
        {
            FirstName = teacher.FirstName,
            LastName = teacher.LastName,
            Email = teacher.Email
        };
        return teacherDto;
    }

    public TeacherResponseDto UpdateTeacher(AddTeacher teacher, string email)
    {
        var updatedTeacher = new Teacher
        {
            FirstName = teacher.FirstName,
            LastName = teacher.LastName,
            Email = teacher.Email
        };
        var result = teacherRepository.UpdateTeacher(updatedTeacher, email);
        var teacherDto = new TeacherResponseDto
        {
            FirstName = result.FirstName,
            LastName = result.LastName,
            Email = result.Email
        };
        return teacherDto;
    }
}
