namespace WebApiStudentManagement.Core.Exceptions;

public class CourseNotFoundException : Exception
{
    public CourseNotFoundException(string title) : base($"Course '{title}' not found.") { }
}
