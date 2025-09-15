namespace StudentManagement.Core.Exceptions;

public class StudentNotFoundException: ApplicationException
{
    public StudentNotFoundException(): base("Student not found with the given id")
    {

    }
}
