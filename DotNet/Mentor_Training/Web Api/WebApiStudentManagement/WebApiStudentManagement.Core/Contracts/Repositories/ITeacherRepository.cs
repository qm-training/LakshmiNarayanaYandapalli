namespace WebApiStudentManagement.Core.Contracts.Repositories;

public interface ITeacherRepository
{
    Task<List<Teacher>> GetAllTeachersAsync();
    Task<Teacher> GetTeacherByEmailAsync(string email);
    Task<string> AddTeacherAsync(Teacher teacher);
    Task<Teacher> UpdateTeacherAsync(Teacher teacher, string email);
    Task<string> DeleteTeacherAsync(string email);
    Task<List<Course>> GetCoursesOfTeacherAsync(string email);
}
