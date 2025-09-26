namespace WebApiStudentManagement.Api.Controllers;

/// <summary>
/// Handles teacher-related operations such as retrieving,
/// creating, updating, deleting, and fetching courses taught by teachers.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class TeacherController(ITeacherService teacherService, ILogger<TeacherController> logger) : ControllerBase
{
    private readonly ITeacherService _teacherService = teacherService;
    private readonly ILogger<TeacherController> _logger = logger;

    /// <summary>
    /// Retrieves all teachers in the system.
    /// </summary>
    /// <returns>A list of all teachers.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllTeachers()
    {
        var teachers = await _teacherService.GetAllTeachersAsync();
        return Ok(teachers);
    }

    /// <summary>
    /// Retrieves a teacher by their email address.
    /// </summary>
    /// <param name="email">The email of the teacher.</param>
    /// <returns>The teacher if found; otherwise, 404 Not Found.</returns>
    [HttpGet("{email}")]
    public async Task<IActionResult> GetTeacherByEmail(string email)
    {
        var teacher = await _teacherService.GetTeacherByEmailAsync(email);
        if (teacher == null)
        {
            _logger.LogWarning("Teacher with email {Email} not found", email);
            return NotFound($"Teacher with email {email} not found.");
        }
        return Ok(teacher);
    }

    /// <summary>
    /// Adds a new teacher to the system.
    /// </summary>
    /// <param name="teacher">The teacher details.</param>
    /// <returns>A 201 Created response with the created teacher.</returns>
    [HttpPost]
    public async Task<IActionResult> AddTeacher(TeacherVm teacher)
    {
        var result = await _teacherService.AddTeacherAsync(teacher);
        return CreatedAtAction(nameof(GetTeacherByEmail), new { email = teacher.Email }, result);
    }

    /// <summary>
    /// Updates an existing teacher's details by email.
    /// </summary>
    /// <param name="teacher">The updated teacher details.</param>
    /// <param name="email">The email of the teacher to update.</param>
    /// <returns>The updated teacher; otherwise, 404 Not Found.</returns>
    [HttpPut("{email}")]
    public async Task<IActionResult> UpdateTeacher(TeacherVm teacher, string email)
    {
        var updatedTeacher = await _teacherService.UpdateTeacherAsync(teacher, email);
        if (updatedTeacher == null)
        {
            _logger.LogWarning("Teacher with email {Email} not found for update", email);
            return NotFound($"Teacher with email {email} not found.");
        }
        return Ok(updatedTeacher);
    }

    /// <summary>
    /// Deletes a teacher by their email address.
    /// </summary>
    /// <param name="email">The email of the teacher to delete.</param>
    /// <returns>A success message if deleted; otherwise, 404 Not Found.</returns>
    [HttpDelete("{email}")]
    public async Task<IActionResult> DeleteTeacher(string email)
    {
        var result = await _teacherService.DeleteTeacherAsync(email);
        if (result.Contains("not found"))
        {
            _logger.LogWarning("Teacher with email {Email} not found for deletion", email);
            return NotFound(result);
        }
        return Ok(result);
    }

    /// <summary>
    /// Retrieves all courses assigned to a specific teacher.
    /// </summary>
    /// <param name="email">The email of the teacher.</param>
    /// <returns>A list of courses if found; otherwise, 404 Not Found.</returns>
    [HttpGet("{email}/courses")]
    public async Task<IActionResult> GetCoursesOfTeacher(string email)
    {
        var courses = await _teacherService.GetCoursesOfTeacherAsync(email);
        if (courses == null || !courses.Any())
        {
            _logger.LogWarning("No courses found for teacher {Email}", email);
            return NotFound($"No courses found for teacher with email {email}.");
        }
        return Ok(courses);
    }
}
