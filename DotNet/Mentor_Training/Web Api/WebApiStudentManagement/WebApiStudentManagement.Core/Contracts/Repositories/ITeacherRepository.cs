namespace WebApiStudentManagement.Core.Contracts.Repositories;

public interface ITeacherRepository
{
    Task<List<Teacher>> GetAllTeachers();
    Task<Teacher> GetTeacherByEmail(string email);
    Task<string> AddTeacher(Teacher teacher);
    Task<Teacher> UpdateTeacher(Teacher teacher, string email);
    Task<string> DeleteTeacher(string email);
    Task<List<Course>> GetCoursesOfTeacher(string email);
}
