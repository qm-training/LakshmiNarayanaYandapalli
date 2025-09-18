namespace WebApiStudentManagement.Core.Contracts.Services;

public interface ITeacherService
{
    public List<TeacherResponseDto> GetAllTeachers();
    public TeacherResponseDto GetTeacherByEmail(string email);
    public string AddTeacher(AddTeacher teacher);
    public TeacherResponseDto UpdateTeacher(AddTeacher teacher, string email);
    public string DeleteTeacher(string email);
    public List<CourseResponseDto> GetCoursesOfTeacher(string email);
}
