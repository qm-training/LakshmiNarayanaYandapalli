namespace WebApiStudentManagement.Core.Contracts.Repositories;

public interface IStudentRepository
{
    Task<List<Student>> GetAllStudents();
    Task<Student> GetStudentByEmail(string email);
    Task<string> AddStudent(Student student);
    Task<Student> UpdateStudent(Student student, string email);
    Task<string> EnrollStudentInCourse(string studentEmail, string courseTitle);
    Task<string> DeleteStudent(string email);
    Task<List<Course>> GetCoursesOfStudent(string email);
}
