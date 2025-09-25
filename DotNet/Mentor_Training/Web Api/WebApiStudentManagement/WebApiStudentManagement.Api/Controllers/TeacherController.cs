namespace WebApiStudentManagement.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TeacherController(ITeacherService teacherService, ILogger<TeacherController> logger) : ControllerBase
{
    private readonly ITeacherService _teacherService = teacherService;
    private readonly ILogger<TeacherController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> GetAllTeachers()
    {
        _logger.LogInformation("Fetching all teachers");
        var teachers = await _teacherService.GetAllTeachers();
        _logger.LogInformation("Retrieved {Count} teachers", teachers.Count);
        return Ok(teachers);
    }

    [HttpGet("{email}")]
    public async Task<IActionResult> GetTeacherByEmail(string email)
    {
        _logger.LogInformation("Fetching teacher with email {Email}", email);
        var teacher = await _teacherService.GetTeacherByEmail(email);
        if (teacher == null)
        {
            _logger.LogWarning("Teacher with email {Email} not found", email);
            return NotFound($"Teacher with email {email} not found.");
        }
        _logger.LogInformation("Successfully retrieved teacher {Email}", email);
        return Ok(teacher);
    }

    [HttpPost]
    public async Task<IActionResult> AddTeacher(AddTeacher teacher)
    {
        _logger.LogInformation("Adding new teacher with email {Email}", teacher.Email);
        var result = await _teacherService.AddTeacher(teacher);
        _logger.LogInformation("Teacher {Email} added successfully", teacher.Email);
        return CreatedAtAction(nameof(GetTeacherByEmail), new { email = teacher.Email }, result);
    }

    [HttpPut("{email}")]
    public async Task<IActionResult> UpdateTeacher(AddTeacher teacher, string email)
    {
        _logger.LogInformation("Updating teacher with email {Email}", email);
        var updatedTeacher = await _teacherService.UpdateTeacher(teacher, email);
        if (updatedTeacher == null)
        {
            _logger.LogWarning("Teacher with email {Email} not found for update", email);
            return NotFound($"Teacher with email {email} not found.");
        }
        _logger.LogInformation("Teacher {Email} updated successfully", email);
        return Ok(updatedTeacher);
    }

    [HttpDelete("{email}")]
    public async Task<IActionResult> DeleteTeacher(string email)
    {
        _logger.LogInformation("Deleting teacher with email {Email}", email);
        var result = await _teacherService.DeleteTeacher(email);
        if (result.Contains("not found"))
        {
            _logger.LogWarning("Teacher with email {Email} not found for deletion", email);
            return NotFound(result);
        }
        _logger.LogInformation("Teacher {Email} deleted successfully", email);
        return Ok(result);
    }

    [HttpGet("{email}/courses")]
    public async Task<IActionResult> GetCoursesOfTeacher(string email)
    {
        _logger.LogInformation("Fetching courses for teacher {Email}", email);
        var courses = await _teacherService.GetCoursesOfTeacher(email);
        if (courses == null || !courses.Any())
        {
            _logger.LogWarning("No courses found for teacher {Email}", email);
            return NotFound($"No courses found for teacher with email {email}.");
        }
        _logger.LogInformation("Retrieved {Count} courses for teacher {Email}", courses.Count, email);
        return Ok(courses);
    }
}
