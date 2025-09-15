namespace StudentManagement.Core.Contracts.Services;

public interface ICourseService
{
    void AddCourse();
    void DeleteCourse();
    void UpdateCourse();
    void GetCourse();
    void GetAllCourses();
}
