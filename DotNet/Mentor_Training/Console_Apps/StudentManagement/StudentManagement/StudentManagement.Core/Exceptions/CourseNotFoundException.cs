namespace StudentManagement.Core.Exceptions;

public class CourseNotFoundException: ApplicationException
{
    public CourseNotFoundException(): base("Course not found with the given id")
    {
    }
}
