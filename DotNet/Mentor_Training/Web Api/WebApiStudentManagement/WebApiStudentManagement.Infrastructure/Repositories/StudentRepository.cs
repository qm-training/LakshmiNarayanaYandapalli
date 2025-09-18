namespace WebApiStudentManagement.Infrastructure.Repositories;

public class StudentRepository(WebApiStudentManagementDbContext context) : IStudentRepository
{
    public string AddStudent(Student student)
    {
        context.Students.Add(student);
        context.SaveChanges();
        return "Student added successfully";
    }

    public string DeleteStudent(string email)
    {
        var student = context.Students.FirstOrDefault(s => s.Email == email);
        if (student == null)
        {
            return "Student not found";
        }
        context.Students.Remove(student);
        context.SaveChanges();
        return "Student deleted successfully";
    }

    public List<Student> GetAllStudents()
    {
        return context.Students.ToList();
    }

    public Student GetStudentByEmail(string email)
    {
        return context.Students.FirstOrDefault(s => s.Email == email);
    }

    public Student UpdateStudent(Student student, string email)
    {
        var existingStudent = GetStudentByEmail(email);
        if (existingStudent == null)
        {
            return null;
        }
        existingStudent.FirstName = student.FirstName;
        existingStudent.LastName = student.LastName;
        existingStudent.Email = student.Email;
        context.SaveChanges();
        return existingStudent;
    }

    public string EnrollStudentInCourse(string studentEmail, string courseTitle)
    {
        var student = GetStudentByEmail(studentEmail);
        if (student == null)
        {
            return "Student not found";
        }

        var course = context.Courses.FirstOrDefault(c => c.Title == courseTitle);
        if (course == null)
        {
            return "Course not found";
        }

        bool alreadyEnrolled = context.Enrollments
                    .Any(e => e.StudentId == student.Id && e.CourseId == course.Id);
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

        context.Enrollments.Add(enrollment);
        context.SaveChanges();
        return "Student enrolled in course successfully";
    }

    public List<Course> GetCoursesOfStudent(string email)
    {
        var student = GetStudentByEmail(email);
        if (student == null)
        {
            return null;
        }
        var courses = context.Enrollments
            .Where(e => e.StudentId == student.Id)
            .Select(e => e.Course)
            .ToList();
        return courses;
    }
}
