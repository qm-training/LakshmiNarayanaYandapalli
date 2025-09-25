namespace WebApiStudentManagement.Core.Exceptions;

public class StudentNotFoundException : Exception
{
    public StudentNotFoundException(string email) : base ($"Student with email '{email}' not found.") { }
}
