namespace WebApiStudentManagement.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudentController(IStudentService studentService, ILogger<StudentController> logger) : ControllerBase
{
    private readonly IStudentService _studentService = studentService;
    private readonly ILogger<StudentController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> GetAllStudents()
    {
        _logger.LogInformation("Fetching all students");
        var students = await _studentService.GetAllStudents();
        _logger.LogInformation("Retrieved {Count} students", students.Count());
        return Ok(students);
    }

    [HttpGet("{email}")]
    public async Task<IActionResult> GetStudentByEmail(string email)
    {
        _logger.LogInformation("Fetching student with email {Email}", email);
        var student = await _studentService.GetStudentByEmail(email);
        if (student == null)
        {
            _logger.LogWarning("Student with email {Email} not found", email);
            return NotFound("Student not found");
        }
        _logger.LogInformation("Successfully retrieved student {Email}", email);
        return Ok(student);
    }

    [HttpPost]
    public async Task<IActionResult> AddStudent(AddStudent student)
    {
        _logger.LogInformation("Adding new student with email {Email}", student.Email);
        var result = await _studentService.AddStudent(student);
        _logger.LogInformation("Student {Email} added successfully", student.Email);
        return Ok(result);
    }

    [HttpPut("{email}")]
    public async Task<IActionResult> UpdateStudent(AddStudent student, string email)
    {
        _logger.LogInformation("Updating student with email {Email}", email);
        var updatedStudent = await _studentService.UpdateStudent(student, email);
        if (updatedStudent == null)
        {
            _logger.LogWarning("Student with email {Email} not found for update", email);
            return NotFound("Student not found");
        }
        _logger.LogInformation("Student {Email} updated successfully", email);
        return Ok(updatedStudent);
    }

    [HttpDelete("{email}")]
    public async Task<IActionResult> DeleteStudent(string email)
    {
        _logger.LogInformation("Deleting student with email {Email}", email);
        var result = await _studentService.DeleteStudent(email);
        if (result == "Student not found")
        {
            _logger.LogWarning("Student with email {Email} not found for deletion", email);
            return NotFound(result);
        }
        _logger.LogInformation("Student {Email} deleted successfully", email);
        return Ok(result);
    }

    [HttpPost("{studentEmail}/enroll/{courseTitle}")]
    public async Task<IActionResult> EnrollStudentInCourse(string studentEmail, string courseTitle)
    {
        _logger.LogInformation("Enrolling student {Email} in course {CourseTitle}", studentEmail, courseTitle);
        var result = await _studentService.EnrollStudentInCourse(studentEmail, courseTitle);

        if (result == "Student or Course not found")
        {
            _logger.LogWarning("Failed to enroll student {Email} in course {CourseTitle}: not found", studentEmail, courseTitle);
            return NotFound(result);
        }
        _logger.LogInformation("Student {Email} enrolled in course {CourseTitle} successfully", studentEmail, courseTitle);
        return Ok(result);
    }

    [HttpGet("{email}/courses")]
    public async Task<IActionResult> GetCoursesOfStudent(string email)
    {
        _logger.LogInformation("Fetching courses for student {Email}", email);
        var courses = await _studentService.GetCoursesOfStudent(email);
        if (courses == null || courses.Count == 0)
        {
            _logger.LogWarning("No courses found for student {Email}", email);
            return NotFound("Student not found or no courses enrolled");
        }
        _logger.LogInformation("Retrieved {Count} courses for student {Email}", courses.Count(), email);
        return Ok(courses);
    }
}
