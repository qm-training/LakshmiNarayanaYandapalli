namespace WebApiStudentManagement.Core.Exceptions;

public class StudentManagementException: ApplicationException
{
    public StudentManagementException(string messgae) : base(messgae) { }
}
