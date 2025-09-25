namespace WebApiStudentManagement.Core.Contracts.Services;

public interface ICourseService
{
    Task<List<CourseResponseDto>> GetAllCourses();
    Task<CourseResponseDto> GetCourseByTitle(string title);
    Task<string> AddCourse(AddCourse course);
    Task<CourseResponseDto> UpdateCourse(AddCourse course, string title);
    Task<string> DeleteCourse(string email);
    Task<List<StudentResponseDto>> GetStudentsInCourse(string title);
    Task<TeacherResponseDto> GetTeacherOfCourse(string title);
}
