namespace WebApiStudentManagement.Core.Contracts.Repositories;

public interface ITeacherRepository
{
    List<Teacher> GetAllTeachers();
    Teacher GetTeacherByEmail(string email);
    string AddTeacher(Teacher teacher);
    Teacher UpdateTeacher(Teacher teacher, string email);
    string DeleteTeacher(string email);
    List<Course> GetCoursesOfTeacher(string email);
}
