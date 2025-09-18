namespace WebApiStudentManagement.Core.Contracts.Repositories;

public interface ICourseRepository
{
    List<Course> GetAllCourses();
    Course GetCourseByTitle(string title);
    string AddCourse(Course course);
    Course UpdateCourse(Course course, string title);
    string DeleteCourse(string email);
    List<Student> GetStudentsInCourse(string title);
    Teacher GetTeacherOfCourse(string title);

}
