namespace WebApiStudentManagement.Core.Exceptions;

public class TeacherNotFoundException: Exception
{
    public TeacherNotFoundException(string email) : base($"Teacher with email '{email}' not found.") { }
}
