namespace WebApiStudentManagement.Infrastructure.Services;

public class CourseService(ICourseRepository courseRepository) : ICourseService
{
    public string AddCourse(AddCourse course)
    {
        var newCourse = new Course()
        {
            Title = course.Title
        };
        return courseRepository.AddCourse(newCourse);
    }

    public string DeleteCourse(string email)
    {
        return courseRepository.DeleteCourse(email);
    }

    public List<CourseResponseDto> GetAllCourses()
    {
        var courses = courseRepository.GetAllCourses();
        var courseDtos = courses.Select(course => new CourseResponseDto
        {
            Title = course.Title,
        }).ToList();
        return courseDtos;
    }

    public CourseResponseDto GetCourseByTitle(string title)
    {
        var course = courseRepository.GetCourseByTitle(title);
        var courseDto = new CourseResponseDto()
        {
            Title = course.Title,
        };
        return courseDto;
    }

    public List<StudentResponseDto> GetStudentsInCourse(string title)
    {
        var students = courseRepository.GetStudentsInCourse(title);
        var studentDtos = students.Select(student => new StudentResponseDto
        {
            FirstName = student.FirstName,
            LastName = student.LastName,
            Email = student.Email
        }).ToList();
        return studentDtos;
    }

    public TeacherResponseDto GetTeacherOfCourse(string title)
    {
        var teacher = courseRepository.GetTeacherOfCourse(title);
        var teacherDto = new TeacherResponseDto()
        {
            FirstName = teacher.FirstName,
            LastName = teacher.LastName,
            Email = teacher.Email
        };
        return teacherDto;
    }

    public CourseResponseDto UpdateCourse(AddCourse course, string title)
    {
        var updatedCourse = new Course()
        {
            Title = course.Title
        };
        var result = courseRepository.UpdateCourse(updatedCourse, title);
        var courseDto = new CourseResponseDto()
        {
            Title = result.Title,
        };
        return courseDto;
    }
}
