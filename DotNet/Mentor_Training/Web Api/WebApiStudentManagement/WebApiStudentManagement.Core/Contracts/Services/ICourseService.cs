namespace WebApiStudentManagement.Core.Contracts.Services;

public interface ICourseService
{
    List<CourseResponseDto> GetAllCourses();
    CourseResponseDto GetCourseByTitle(string title);
    string AddCourse(AddCourse course);
    CourseResponseDto UpdateCourse(AddCourse course, string title);
    string DeleteCourse(string email);
    List<StudentResponseDto> GetStudentsInCourse(string title);
    TeacherResponseDto GetTeacherOfCourse(string title);
}
