namespace WebApiStudentManagement.Core.Contracts.Services;

public interface ITeacherService
{
    Task<List<TeacherResponseDto>> GetAllTeachers();
    Task<TeacherResponseDto> GetTeacherByEmail(string email);
    Task<string> AddTeacher(AddTeacher teacher);
    Task<TeacherResponseDto> UpdateTeacher(AddTeacher teacher, string email);
    Task<string> DeleteTeacher(string email);
    Task<List<CourseResponseDto>> GetCoursesOfTeacher(string email);
}
