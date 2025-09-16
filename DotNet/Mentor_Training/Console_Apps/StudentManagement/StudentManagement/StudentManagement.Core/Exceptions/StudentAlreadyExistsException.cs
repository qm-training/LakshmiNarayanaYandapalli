namespace StudentManagement.Core.Exceptions;

public class StudentAlreadyExistsException : ApplicationException
{
    public StudentAlreadyExistsException() : base("Student already exists with the given id")
    {
    }
}