namespace WebApiStudentManagement.Core.Contracts.Repositories;

public interface IStudentRepository
{
    Task<List<Student>> GetAllStudentsAsync();
    Task<Student> GetStudentByEmailAsync(string email);
    Task<string> AddStudentAsync(Student student);
    Task<Student> UpdateStudentAsync(Student student, string email);
    Task<string> EnrollStudentInCourseAsync(string studentEmail, string courseTitle);
    Task<string> DeleteStudentAsync(string email);
    Task<List<Course>> GetCoursesOfStudentAsync(string email);
}
