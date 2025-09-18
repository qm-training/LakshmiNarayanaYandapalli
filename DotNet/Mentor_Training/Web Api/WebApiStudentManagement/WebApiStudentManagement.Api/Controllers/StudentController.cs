namespace WebApiStudentManagement.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudentController(IStudentService studentService, ILogger<StudentController> logger) : ControllerBase
{
    private readonly ILogger<StudentController> _logger = logger;
    [HttpGet]
    public IActionResult GetAllStudents()
    {
        _logger.LogInformation("Fetching all students");
        var students = studentService.GetAllStudents();
        _logger.LogInformation("Retrieved {Count} students", students.Count());
        return Ok(students);
    }

    [HttpGet("{email}")]
    public IActionResult GetStudentByEmail(string email)
    {
        _logger.LogInformation("Fetching student with email {Email}", email);
        var student = studentService.GetStudentByEmail(email);
        if (student == null)
        {
            _logger.LogWarning("Student with email {Email} not found", email);
            return NotFound("Student not found");
        }
        _logger.LogInformation("Successfully retrieved student {Email}", email);
        return Ok(student);
    }

    [HttpPost]
    public IActionResult AddStudent(AddStudent student)
    {
        _logger.LogInformation("Adding new student with email {Email}", student.Email);
        var result = studentService.AddStudent(student);

        _logger.LogInformation("Student {Email} added successfully", student.Email);
        return Ok(result);
    }

    [HttpPut("{email}")]
    public IActionResult UpdateStudent(AddStudent student, string email)
    {
        _logger.LogInformation("Updating student with email {Email}", email);
        var updatedStudent = studentService.UpdateStudent(student, email);
        if (updatedStudent == null)
        {
            _logger.LogWarning("Student with email {Email} not found for update", email);
            return NotFound("Student not found");
        }
        _logger.LogInformation("Student {Email} updated successfully", email);
        return Ok(updatedStudent);
    }

    [HttpDelete("{email}")]
    public IActionResult DeleteStudent(string email)
    {
        _logger.LogInformation("Deleting student with email {Email}", email);
        var result = studentService.DeleteStudent(email);
        if (result == "Student not found")
        {
            _logger.LogWarning("Student with email {Email} not found for deletion", email);
            return NotFound(result);
        }
        _logger.LogInformation("Student {Email} deleted successfully", email);
        return Ok(result);
    }

    [HttpPost("{studentEmail}/enroll/{courseTitle}")]
    public IActionResult EnrollStudentInCourse(string studentEmail, string courseTitle)
    {
        _logger.LogInformation("Enrolling student {Email} in course {CourseTitle}", studentEmail, courseTitle);
        var result = studentService.EnrollStudentInCourse(studentEmail, courseTitle);

        if (result == "Student or Course not found")
        {
            _logger.LogWarning("Failed to enroll student {Email} in course {CourseTitle}: not found", studentEmail, courseTitle);
            return NotFound(result);
        }
        _logger.LogInformation("Student {Email} enrolled in course {CourseTitle} successfully", studentEmail, courseTitle);
        return Ok(result);
    }

    [HttpGet("{email}/courses")]
    public IActionResult GetCoursesOfStudent(string email)
    {
        _logger.LogInformation("Fetching courses for student {Email}", email);
        var courses = studentService.GetCoursesOfStudent(email);
        if (courses == null)
        {
            _logger.LogWarning("No courses found for student {Email}", email);
            return NotFound("Student not found");
        }
        _logger.LogInformation("Retrieved {Count} courses for student {Email}", courses.Count(), email);
        return Ok(courses);
    }
}
