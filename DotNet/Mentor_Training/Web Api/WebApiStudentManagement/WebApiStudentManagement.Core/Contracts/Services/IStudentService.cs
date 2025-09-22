namespace WebApiStudentManagement.Core.Contracts.Services;

public interface IStudentService
{
    Task<List<StudentResponseDto>> GetAllStudents();
    Task<StudentResponseDto> GetStudentByEmail(string email);
    Task<string> AddStudent(AddStudent student);
    Task<StudentResponseDto> UpdateStudent(AddStudent student, string email);
    Task<string> DeleteStudent(string email);
    Task<string> EnrollStudentInCourse(string studentEmail, string courseTitle);
    Task<List<CourseResponseDto>> GetCoursesOfStudent(string email);
}
