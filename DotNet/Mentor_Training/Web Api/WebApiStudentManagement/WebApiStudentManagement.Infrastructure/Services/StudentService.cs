namespace WebApiStudentManagement.Infrastructure.Services;

public class StudentService(IStudentRepository studentRepository) : IStudentService
{
    public string AddStudent(AddStudent student)
    {
        var newStudent = new Student()
        {
            FirstName = student.FirstName,
            LastName = student.LastName,
            Email = student.Email
        };
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
        var studentDtos = students.Select(student => new StudentResponseDto
        {
            FirstName = student.FirstName,
            LastName = student.LastName,
            Email = student.Email
        }).ToList();
        return studentDtos;
    }

    public List<CourseResponseDto> GetCoursesOfStudent(string email)
    {
        var courses = studentRepository.GetCoursesOfStudent(email);
        var courseDtos = courses.Select(course => new CourseResponseDto
        {
            Title = course.Title,
        }).ToList();
        return courseDtos;
    }

    public StudentResponseDto GetStudentByEmail(string email)
    {
        var student = studentRepository.GetStudentByEmail(email);
        if (student == null)
        {
            return null;
        }
        else
        {
            var studentDto = new StudentResponseDto()
            {
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email
            };
            return studentDto;
        }
    }

    public StudentResponseDto UpdateStudent(AddStudent student, string email)
    {
        var updatedStudent = new Student()
        {
            FirstName = student.FirstName,
            LastName = student.LastName,
            Email = student.Email
        };
        var result = studentRepository.UpdateStudent(updatedStudent, email);
        var studentDto = new StudentResponseDto()
        {
            FirstName = result.FirstName,
            LastName = result.LastName,
            Email = result.Email
        };
        return studentDto;
    }
}
