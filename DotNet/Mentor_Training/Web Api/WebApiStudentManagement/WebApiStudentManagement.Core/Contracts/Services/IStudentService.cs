namespace WebApiStudentManagement.Core.Contracts.Services;

public interface IStudentService
{
    List<StudentResponseDto> GetAllStudents();
    StudentResponseDto GetStudentByEmail(string email);
    string AddStudent(AddStudent student);
    StudentResponseDto UpdateStudent(AddStudent student, string email);
    string DeleteStudent(string email);
    string EnrollStudentInCourse(string studentEmail, string courseTitle);
    List<CourseResponseDto> GetCoursesOfStudent(string email);
}
