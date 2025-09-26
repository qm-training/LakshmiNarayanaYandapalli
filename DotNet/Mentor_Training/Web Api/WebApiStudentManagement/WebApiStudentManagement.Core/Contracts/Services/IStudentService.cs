using WebApiStudentManagement.Core.Vms;

namespace WebApiStudentManagement.Core.Contracts.Services;

public interface IStudentService
{
    Task<List<StudentResponseDto>> GetAllStudentsAsync();
    Task<StudentResponseDto> GetStudentByEmailAsync(string email);
    Task<string> AddStudentAsync(StudentVm student);
    Task<StudentResponseDto> UpdateStudentAsync(StudentVm student, string email);
    Task<string> DeleteStudentAsync(string email);
    Task<string> EnrollStudentInCourseAsync(string studentEmail, string courseTitle);
    Task<List<CourseResponseDto>> GetCoursesOfStudentAsync(string email);
}
