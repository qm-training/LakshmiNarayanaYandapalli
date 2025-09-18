namespace WebApiStudentManagement.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CourseController(ICourseService courseService) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAllCourses()
    {
        var courses = courseService.GetAllCourses();
        return Ok(courses);
    }

    [HttpGet("{title}")]
    public IActionResult GetCourseByTitle(string title)
    {
        var course = courseService.GetCourseByTitle(title);
        if (course == null)
        {
            return NotFound("Course not found");
        }
        return Ok(course);
    }

    [HttpPost]
    public IActionResult AddCourse(AddCourse course)
    {
        var result = courseService.AddCourse(course);
        return Ok(result);
    }

    [HttpPut("{title}")]
    public IActionResult UpdateCourse(AddCourse course, string title)
    {
        var updatedCourse = courseService.UpdateCourse(course, title);
        if (updatedCourse == null)
        {
            return NotFound("Course not found");
        }
        return Ok(updatedCourse);
    }

    [HttpDelete("{title}")]
    public IActionResult DeleteCourse(string title)
    {
        var result = courseService.DeleteCourse(title);
        if (result == null)
        {
            return NotFound("Course not found");
        }
        return Ok(result);
    }

    [HttpGet("{title}/students")]
    public IActionResult GetStudentsInCourse(string title)
    {
        var students = courseService.GetStudentsInCourse(title);
        if (students == null)
        {
            return NotFound("Course not found");
        }
        return Ok(students);
    }

    [HttpGet("{title}/teacher")]
    public IActionResult GetTeacherOfCourse(string title)
    {
        var teacher = courseService.GetTeacherOfCourse(title);
        if (teacher == null)
        {
            return NotFound("Course not found");
        }
        return Ok(teacher);
    }

}
