namespace WebApiStudentManagement.Core.Contracts.Repositories;

public interface ICourseRepository
{
    Task<List<Course>> GetAllCoursesAsync();
    Task<Course> GetCourseByTitleAsync(string title);
    Task<string> AddCourseAsync(Course course);
    Task<Course> UpdateCourseAsync(Course course, string title);
    Task<string> DeleteCourseAsync(string email);
    Task<List<Student>> GetStudentsInCourseAsync(string title);
    Task<Teacher> GetTeacherOfCourseAsync(string title);

}
