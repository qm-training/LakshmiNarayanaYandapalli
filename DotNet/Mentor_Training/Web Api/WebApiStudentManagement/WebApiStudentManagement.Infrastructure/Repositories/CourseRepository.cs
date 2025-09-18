namespace WebApiStudentManagement.Infrastructure.Repositories;

public class CourseRepository(WebApiStudentManagementDbContext context) : ICourseRepository
{
    public string AddCourse(Course course)
    {
        context.Courses.Add(course);
        context.SaveChanges();
        return "Course added successfully";
    }

    public string DeleteCourse(string email)
    {
        var course = context.Courses.FirstOrDefault(c => c.Title == email);
        if (course == null)
        {
            return "Course not found";
        }
        context.Courses.Remove(course);
        context.SaveChanges();
        return "Course deleted successfully";
    }

    public List<Course> GetAllCourses()
    {
        return context.Courses.ToList();
    }

    public Course GetCourseByTitle(string title)
    {
        return context.Courses.FirstOrDefault(c => c.Title == title);
    }

    public Course UpdateCourse(Course course, string title)
    {
        var existingCourse = GetCourseByTitle(title);
        if (existingCourse == null)
        {
            return null;
        }
        existingCourse.Title = course.Title;
        context.SaveChanges();
        return existingCourse;
    }

    public List<Student> GetStudentsInCourse(string title)
    {
        var course = context.Courses.FirstOrDefault(c => c.Title == title);
        if (course == null)
        {
            return new List<Student>();
        }
        return context.Enrollments
            .Where(e => e.CourseId == course.Id)
            .Select(e => e.Student)
            .ToList();
    }

    public Teacher GetTeacherOfCourse(string title)
    {
        return context.Courses
     .Include(c => c.Teacher)
     .Where(c => c.Title == title)
     .Select(c => c.Teacher)
     .FirstOrDefault();
    }
}
