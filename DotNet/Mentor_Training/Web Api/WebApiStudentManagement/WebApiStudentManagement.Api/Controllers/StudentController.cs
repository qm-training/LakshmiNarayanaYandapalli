namespace WebApiStudentManagement.Api.Controllers;

/// <summary>
/// Handles student-related operations such as creating, updating,
/// deleting, enrolling in courses, and retrieving student details.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class StudentController(IStudentService studentService, ILogger<StudentController> logger) : ControllerBase
{
    private readonly IStudentService _studentService = studentService;
    private readonly ILogger<StudentController> _logger = logger;

    /// <summary>
    /// Retrieves all students.
    /// </summary>
    /// <returns>A list of all students.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllStudents()
    {
        var students = await _studentService.GetAllStudentsAsync();
        return Ok(students);
    }

    /// <summary>
    /// Retrieves a specific student by email.
    /// </summary>
    /// <param name="email">The student's email address.</param>
    /// <returns>The student if found; otherwise, 404 Not Found.</returns>
    [HttpGet("{email}")]
    public async Task<IActionResult> GetStudentByEmail(string email)
    {
        var student = await _studentService.GetStudentByEmailAsync(email);
        if (student == null)
        {
            _logger.LogWarning("Student with email {Email} not found", email);
            return NotFound("Student not found");
        }
        _logger.LogInformation("Successfully retrieved student {Email}", email);
        return Ok(student);
    }

    /// <summary>
    /// Adds a new student to the system.
    /// </summary>
    /// <param name="student">The student details.</param>
    /// <returns>A success message.</returns>
    [HttpPost]
    public async Task<IActionResult> AddStudent(StudentVm student)
    {
        var result = await _studentService.AddStudentAsync(student);
        return Ok(result);
    }

    /// <summary>
    /// Updates an existing student's details.
    /// </summary>
    /// <param name="student">The updated student details.</param>
    /// <param name="email">The email of the student to update.</param>
    /// <returns>The updated student; otherwise, 404 Not Found.</returns>
    [HttpPut("{email}")]
    public async Task<IActionResult> UpdateStudent(StudentVm student, string email)
    {
        var updatedStudent = await _studentService.UpdateStudentAsync(student, email);
        if (updatedStudent == null)
        {
            _logger.LogWarning("Student with email {Email} not found for update", email);
            return NotFound("Student not found");
        }
        return Ok(updatedStudent);
    }

    /// <summary>
    /// Deletes a student from the system.
    /// </summary>
    /// <param name="email">The student's email address.</param>
    /// <returns>A success or error message.</returns>
    [HttpDelete("{email}")]
    public async Task<IActionResult> DeleteStudent(string email)
    {
        var result = await _studentService.DeleteStudentAsync(email);
        if (result == "Student not found")
        {
            _logger.LogWarning("Student with email {Email} not found for deletion", email);
            return NotFound(result);
        }
        return Ok(result);
    }

    /// <summary>
    /// Enrolls a student in a specific course.
    /// </summary>
    /// <param name="studentEmail">The student's email address.</param>
    /// <param name="courseTitle">The title of the course.</param>
    /// <returns>A success or error message.</returns>
    [HttpPost("{studentEmail}/enroll/{courseTitle}")]
    public async Task<IActionResult> EnrollStudentInCourse(string studentEmail, string courseTitle)
    {
        var result = await _studentService.EnrollStudentInCourseAsync(studentEmail, courseTitle);

        if (result == "Student or Course not found")
        {
            _logger.LogWarning("Failed to enroll student {Email} in course {CourseTitle}: not found", studentEmail, courseTitle);
            return NotFound(result);
        }
        return Ok(result);
    }

    /// <summary>
    /// Retrieves all courses a student is enrolled in.
    /// </summary>
    /// <param name="email">The student's email address.</param>
    /// <returns>A list of courses; otherwise, 404 Not Found.</returns>
    [HttpGet("{email}/courses")]
    public async Task<IActionResult> GetCoursesOfStudent(string email)
    {
        var courses = await _studentService.GetCoursesOfStudentAsync(email);
        if (courses == null || courses.Count == 0)
        {
            _logger.LogWarning("No courses found for student {Email}", email);
            return NotFound("Student not found or no courses enrolled");
        }
        return Ok(courses);
    }
}
