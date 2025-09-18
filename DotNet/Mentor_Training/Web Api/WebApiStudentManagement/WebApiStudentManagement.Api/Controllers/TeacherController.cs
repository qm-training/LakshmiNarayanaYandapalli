namespace WebApiStudentManagement.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TeacherController(ITeacherService teacherService) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAllTeachers()
    {
        var teachers = teacherService.GetAllTeachers();
        return Ok(teachers);
    }

    [HttpGet("{email}")]
    public IActionResult GetTeacherByEmail(string email)
    {
        var teacher = teacherService.GetTeacherByEmail(email);
        if (teacher == null)
        {
            return NotFound($"Teacher with email {email} not found.");
        }
        return Ok(teacher);
    }

    [HttpPost]
    public IActionResult AddTeacher(AddTeacher teacher)
    {
        var result = teacherService.AddTeacher(teacher);
        return CreatedAtAction(nameof(GetTeacherByEmail), new { email = teacher.Email }, result);
    }

    [HttpPut("{email}")]
    public IActionResult UpdateTeacher(AddTeacher teacher, string email)
    {
        var updatedTeacher = teacherService.UpdateTeacher(teacher, email);
        if (updatedTeacher == null)
        {
            return NotFound($"Teacher with email {email} not found.");
        }
        return Ok(updatedTeacher);
    }
    [HttpDelete("{email}")]
    public IActionResult DeleteTeacher(string email)
    {
        var result = teacherService.DeleteTeacher(email);
        if (result.Contains("not found"))
        {
            return NotFound(result);
        }
        return Ok(result);
    }
    [HttpGet("{email}/courses")]
    public IActionResult GetCoursesOfTeacher(string email)
    {
        var courses = teacherService.GetCoursesOfTeacher(email);
        if (courses == null || !courses.Any())
        {
            return NotFound($"No courses found for teacher with email {email}.");
        }
        return Ok(courses);
    }
}
