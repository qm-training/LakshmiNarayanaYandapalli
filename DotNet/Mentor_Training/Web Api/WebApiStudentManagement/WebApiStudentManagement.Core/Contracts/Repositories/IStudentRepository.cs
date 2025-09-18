namespace WebApiStudentManagement.Core.Contracts.Repositories;

public interface IStudentRepository
{
    List<Student> GetAllStudents();
    Student GetStudentByEmail(string email);
    string AddStudent(Student student);
    Student UpdateStudent(Student student, string email);
    string EnrollStudentInCourse(string studentEmail, string courseTitle);
    string DeleteStudent(string email);
    List<Course> GetCoursesOfStudent(string email);
}
