namespace StudentManagement.Core.Contracts.Services
{
    public interface IStudentService
    {
        void AddStudent();
        void DeleteStudent();
        void UpdateStudent();
        void GetStudentById();
        void GetAllStudents();
        void GetStudentsByAge();
        void GetStudentsByCourseName();
        void GetCoursesbyStudentId();
        void AddCoursesToStudent();
    }
}
