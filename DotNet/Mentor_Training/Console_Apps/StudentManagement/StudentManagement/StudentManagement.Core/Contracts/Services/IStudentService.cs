namespace StudentManagement.Core.Contracts.Services
{
    public interface IStudentService
    {
        public void AddStudent();
        public void DeleteStudent();
        public void UpdateStudent();
        public void GetStudentById();
        public void GetAllStudents();
        public void GetStudentsByAge();
        public void GetStudentsByCourseName();
        public void GetCoursesbyStudentId();
        public void AddCoursesToStudent();
    }
}
