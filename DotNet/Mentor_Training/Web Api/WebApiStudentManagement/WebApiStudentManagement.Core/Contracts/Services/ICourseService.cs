using WebApiStudentManagement.Core.Vms;

namespace WebApiStudentManagement.Core.Contracts.Services;

public interface ICourseService
{
    Task<List<CourseResponseDto>> GetAllCoursesAsync();
    Task<CourseResponseDto> GetCourseByTitleAsync(string title);
    Task<string> AddCourseAsync(CourseVm course);
    Task<CourseResponseDto> UpdateCourseAsync(CourseVm course, string title);
    Task<string> DeleteCourseAsync(string email);
    Task<List<StudentResponseDto>> GetStudentsInCourseAsync(string title);
    Task<TeacherResponseDto> GetTeacherOfCourseAsync(string title);
}
