namespace WebApiStudentManagement.Infrastructure.Repositories;

public class CourseRepository(WebApiStudentManagementDbContext context) : ICourseRepository
{
    public async Task<string> AddCourseAsync(Course course)
    {
        await context.Courses.AddAsync(course);
        await context.SaveChangesAsync();
        return "Course added successfully";
    }

    public async Task<string> DeleteCourseAsync(string email)
    {
        var course = await context.Courses.FirstOrDefaultAsync(c => c.Title == email);
        if (course == null)
        {
            return "Course not found";
        }
        context.Courses.Remove(course);
        await context.SaveChangesAsync();
        return "Course deleted successfully";
    }

    public async Task<List<Course>> GetAllCoursesAsync()
    {
        return await context.Courses.ToListAsync();
    }

    public async Task<Course> GetCourseByTitleAsync(string title)
    {
        return await context.Courses.FirstOrDefaultAsync(c => c.Title == title);
    }

    public async Task<Course> UpdateCourseAsync(Course course, string title)
    {
        var existingCourse = await GetCourseByTitleAsync(title);
        if (existingCourse == null)
        {
            return null;
        }
        existingCourse.Title = course.Title;
        await context.SaveChangesAsync();
        return existingCourse;
    }

    public async Task<List<Student>> GetStudentsInCourseAsync(string title)
    {
        var course = await context.Courses.FirstOrDefaultAsync(c => c.Title == title);
        if (course == null)
        {
            return new List<Student>();
        }
        return await context.Enrollments
            .Where(e => e.CourseId == course.Id)
            .Select(e => e.Student)
            .ToListAsync();
    }

    public async Task<Teacher> GetTeacherOfCourseAsync(string title)
    {
        return await context.Courses
                        .Include(c => c.Teacher)
                        .Where(c => c.Title == title)
                        .Select(c => c.Teacher)
                        .FirstOrDefaultAsync();
    }
}
