namespace StudentManagement.Core.Contracts.Services
{
    public interface ICourseService
    {
        public void AddCourse();
        public void DeleteCourse();
        public void UpdateCourse();
        public void GetCourse();
        public void GetAllCourses();
    }
}
