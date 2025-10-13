namespace WebApiStudentManagement.Core.Models;

public class Course
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int? TeacherId { get; set; }
    public Teacher? Teacher { get; set; }
    public List<Enrollment> Enrollments { get; set; } = new();
}
