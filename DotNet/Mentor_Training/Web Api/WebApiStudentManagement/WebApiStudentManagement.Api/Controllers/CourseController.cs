namespace WebApiStudentManagement.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CourseController(ICourseService courseService, ILogger<CourseController> logger) : ControllerBase
{
    private readonly ICourseService _courseService = courseService;
    private readonly ILogger<CourseController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> GetAllCourses()
    {
        _logger.LogInformation("Fetching all courses");
        var courses = await _courseService.GetAllCourses();
        _logger.LogInformation("Retrieved {Count} courses", courses.Count);
        return Ok(courses);
    }

    [HttpGet("{title}")]
    public async Task<IActionResult> GetCourseByTitle(string title)
    {
        _logger.LogInformation("Fetching course with title {Title}", title);
        var course = await _courseService.GetCourseByTitle(title);
        if (course == null)
        {
            _logger.LogWarning("Course with title {Title} not found", title);
            return NotFound("Course not found");
        }
        _logger.LogInformation("Successfully retrieved course {Title}", title);
        return Ok(course);
    }

    [HttpPost]
    public async Task<IActionResult> AddCourse(AddCourse course)
    {
        _logger.LogInformation("Adding new course with title {Title}", course.Title);
        var result = await _courseService.AddCourse(course);
        _logger.LogInformation("Course {Title} added successfully", course.Title);
        return Ok(result);
    }

    [HttpPut("{title}")]
    public async Task<IActionResult> UpdateCourse(AddCourse course, string title)
    {
        _logger.LogInformation("Updating course with title {Title}", title);
        var updatedCourse = await _courseService.UpdateCourse(course, title);
        if (updatedCourse == null)
        {
            _logger.LogWarning("Course with title {Title} not found for update", title);
            return NotFound("Course not found");
        }
        _logger.LogInformation("Course {Title} updated successfully", title);
        return Ok(updatedCourse);
    }

    [HttpDelete("{title}")]
    public async Task<IActionResult> DeleteCourse(string title)
    {
        _logger.LogInformation("Deleting course with title {Title}", title);
        var result = await _courseService.DeleteCourse(title);
        if (result == null)
        {
            _logger.LogWarning("Course with title {Title} not found for deletion", title);
            return NotFound("Course not found");
        }
        _logger.LogInformation("Course {Title} deleted successfully", title);
        return Ok(result);
    }

    [HttpGet("{title}/students")]
    public async Task<IActionResult> GetStudentsInCourse(string title)
    {
        _logger.LogInformation("Fetching students for course {Title}", title);
        var students = await _courseService.GetStudentsInCourse(title);
        if (students == null || students.Count == 0)
        {
            _logger.LogWarning("No students found for course {Title}", title);
            return NotFound("Course not found or no students enrolled");
        }
        _logger.LogInformation("Retrieved {Count} students for course {Title}", students.Count, title);
        return Ok(students);
    }

    [HttpGet("{title}/teacher")]
    public async Task<IActionResult> GetTeacherOfCourse(string title)
    {
        _logger.LogInformation("Fetching teacher for course {Title}", title);
        var teacher = await _courseService.GetTeacherOfCourse(title);
        if (teacher == null)
        {
            _logger.LogWarning("No teacher found for course {Title}", title);
            return NotFound("Course not found or no teacher assigned");
        }
        _logger.LogInformation("Successfully retrieved teacher for course {Title}", title);
        return Ok(teacher);
    }
}
