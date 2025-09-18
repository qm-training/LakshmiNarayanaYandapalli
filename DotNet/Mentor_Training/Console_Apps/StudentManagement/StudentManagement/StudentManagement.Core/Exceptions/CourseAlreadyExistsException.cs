namespace StudentManagement.Core.Exceptions;

public class CourseAlreadyExistsException : ApplicationException
{
    public CourseAlreadyExistsException() : base("Course already exists with the given id")
    {
    }
}