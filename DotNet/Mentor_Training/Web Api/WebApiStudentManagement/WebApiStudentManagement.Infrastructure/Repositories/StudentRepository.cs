namespace WebApiStudentManagement.Infrastructure.Repositories;

public class StudentRepository(WebApiStudentManagementDbContext context) : IStudentRepository
{
    public async Task<string> AddStudentAsync(Student student)
    {
        await context.Students.AddAsync(student);
        await context.SaveChangesAsync();
        return "Student added successfully";
    }

    public async Task<string> DeleteStudentAsync(string email)
    {
        var student = await context.Students.FirstOrDefaultAsync(s => s.Email == email);
        if (student == null)
        {
            return "Student not found";
        }
        context.Students.Remove(student);
        await context.SaveChangesAsync();
        return "Student deleted successfully";
    }

    public async Task<List<Student>> GetAllStudentsAsync()
    {
        return await context.Students.ToListAsync();
    }

    public async Task<Student?> GetStudentByEmailAsync(string email)
    {
        return await context.Students.FirstOrDefaultAsync(s => s.Email == email);
    }

    public async Task<Student?> UpdateStudentAsync(Student student, string email)
    {
        var existingStudent = await GetStudentByEmailAsync(email);
        if (existingStudent == null)
        {
            return null;
        }

        existingStudent.FirstName = student.FirstName;
        existingStudent.LastName = student.LastName;
        existingStudent.Email = student.Email;

        await context.SaveChangesAsync();
        return existingStudent;
    }

    public async Task<string> EnrollStudentInCourseAsync(string studentEmail, string courseTitle)
    {
        var student = await GetStudentByEmailAsync(studentEmail);
        if (student == null)
        {
            return "Student not found";
        }

        var course = await context.Courses.FirstOrDefaultAsync(c => c.Title == courseTitle);
        if (course == null)
        {
            return "Course not found";
        }

        bool alreadyEnrolled = await context.Enrollments
            .AnyAsync(e => e.StudentId == student.Id && e.CourseId == course.Id);

        if (alreadyEnrolled)
        {
            return "Student is already enrolled in this course";
        }

        var enrollment = new Enrollment
        {
            StudentId = student.Id,
            CourseId = course.Id,
            EnrollmentDate = DateTime.UtcNow
        };

        await context.Enrollments.AddAsync(enrollment);
        await context.SaveChangesAsync();
        return "Student enrolled in course successfully";
    }

    public async Task<List<Course>> GetCoursesOfStudentAsync(string email)
    {
        var student = await GetStudentByEmailAsync(email);
        if (student == null)
        {
            return new List<Course>();
        }

        return await context.Enrollments
            .Where(e => e.StudentId == student.Id)
            .Select(e => e.Course)
            .ToListAsync();
    }
}
