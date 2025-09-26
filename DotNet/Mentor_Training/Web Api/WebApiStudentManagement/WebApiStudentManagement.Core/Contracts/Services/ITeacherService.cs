using WebApiStudentManagement.Core.Vms;

namespace WebApiStudentManagement.Core.Contracts.Services;

public interface ITeacherService
{
    Task<List<TeacherResponseDto>> GetAllTeachersAsync();
    Task<TeacherResponseDto> GetTeacherByEmailAsync(string email);
    Task<string> AddTeacherAsync(TeacherVm teacher);
    Task<TeacherResponseDto> UpdateTeacherAsync(TeacherVm teacher, string email);
    Task<string> DeleteTeacherAsync(string email);
    Task<List<CourseResponseDto>> GetCoursesOfTeacherAsync(string email);
}
