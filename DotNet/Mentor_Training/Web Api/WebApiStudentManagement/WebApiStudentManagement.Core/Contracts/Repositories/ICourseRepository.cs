namespace WebApiStudentManagement.Core.Contracts.Repositories;

public interface ICourseRepository
{
    Task<List<Course>> GetAllCourses();
    Task<Course> GetCourseByTitle(string title);
    Task<string> AddCourse(Course course);
    Task<Course> UpdateCourse(Course course, string title);
    Task<string> DeleteCourse(string email);
    Task<List<Student>> GetStudentsInCourse(string title);
    Task<Teacher> GetTeacherOfCourse(string title);

}
