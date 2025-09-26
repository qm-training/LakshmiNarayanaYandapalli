namespace WebApiStudentManagement.Api.Controllers;

/// <summary>
/// Handles course-related API endpoints such as retrieving,
/// creating, updating, and deleting courses, as well as managing
/// related students and teachers.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class CourseController(ICourseService courseService, ILogger<CourseController> logger) : ControllerBase
{
    private readonly ICourseService _courseService = courseService;
    private readonly ILogger<CourseController> _logger = logger;

    /// <summary>
    /// Retrieves all courses available in the system.
    /// </summary>
    /// <returns>A list of all courses.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllCourses()
    {
        var courses = await _courseService.GetAllCoursesAsync();
        return Ok(courses);
    }

    /// <summary>
    /// Retrieves a specific course by its title.
    /// </summary>
    /// <param name="title">The title of the course to fetch.</param>
    /// <returns>The course if found; otherwise, 404 Not Found.</returns>
    [HttpGet("{title}")]
    public async Task<IActionResult> GetCourseByTitle(string title)
    {
        var course = await _courseService.GetCourseByTitleAsync(title);
        if (course == null)
        {
            _logger.LogWarning("Course with title {Title} not found", title);
            return NotFound("Course not found");
        }
        return Ok(course);
    }

    /// <summary>
    /// Creates a new course.
    /// </summary>
    /// <param name="course">The course data to create.</param>
    /// <returns>A success message or validation error.</returns>
    [HttpPost]
    public async Task<IActionResult> AddCourse(CourseVm course)
    {
        var result = await _courseService.AddCourseAsync(course);
        return Ok(result);
    }

    /// <summary>
    /// Updates an existing course by its title.
    /// </summary>
    /// <param name="course">The updated course data.</param>
    /// <param name="title">The title of the course to update.</param>
    /// <returns>The updated course if found; otherwise, 404 Not Found.</returns>
    [HttpPut("{title}")]
    public async Task<IActionResult> UpdateCourse(CourseVm course, string title)
    {
        var updatedCourse = await _courseService.UpdateCourseAsync(course, title);
        if (updatedCourse == null)
        {
            _logger.LogWarning("Course with title {Title} not found for update", title);
            return NotFound("Course not found");
        }
        return Ok(updatedCourse);
    }

    /// <summary>
    /// Deletes a course by its title.
    /// </summary>
    /// <param name="title">The title of the course to delete.</param>
    /// <returns>A success message if deleted; otherwise, 404 Not Found.</returns>
    [HttpDelete("{title}")]
    public async Task<IActionResult> DeleteCourse(string title)
    {
        var result = await _courseService.DeleteCourseAsync(title);
        if (result == null)
        {
            _logger.LogWarning("Course with title {Title} not found for deletion", title);
            return NotFound("Course not found");
        }
        return Ok(result);
    }

    /// <summary>
    /// Retrieves all students enrolled in a given course.
    /// </summary>
    /// <param name="title">The title of the course.</param>
    /// <returns>A list of students if found; otherwise, 404 Not Found.</returns>
    [HttpGet("{title}/students")]
    public async Task<IActionResult> GetStudentsInCourse(string title)
    {
        var students = await _courseService.GetStudentsInCourseAsync(title);
        if (students == null || students.Count == 0)
        {
            return NotFound("Course not found or no students enrolled");
        }
        return Ok(students);
    }

    /// <summary>
    /// Retrieves the teacher assigned to a given course.
    /// </summary>
    /// <param name="title">The title of the course.</param>
    /// <returns>The teacher if found; otherwise, 404 Not Found.</returns>
    [HttpGet("{title}/teacher")]
    public async Task<IActionResult> GetTeacherOfCourse(string title)
    {
        var teacher = await _courseService.GetTeacherOfCourseAsync(title);
        if (teacher == null)
        {
            _logger.LogWarning("No teacher found for course {Title}", title);
            return NotFound("Course not found or no teacher assigned");
        }
        return Ok(teacher);
    }
}
