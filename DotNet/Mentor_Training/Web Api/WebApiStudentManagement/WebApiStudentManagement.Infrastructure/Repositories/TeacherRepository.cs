namespace WebApiStudentManagement.Infrastructure.Repositories;

public class TeacherRepository(WebApiStudentManagementDbContext context) : ITeacherRepository
{
    public string AddTeacher(Teacher teacher)
    {
        context.Teachers.Add(teacher);
        context.SaveChanges();
        return "Teacher added successfully";
    }

    public string DeleteTeacher(string email)
    {
        var teacher = context.Teachers.FirstOrDefault(t => t.Email == email);
        if (teacher == null)
        {
            return "Teacher not found";
        }
        context.Teachers.Remove(teacher);
        context.SaveChanges();
        return "Teacher deleted successfully";
    }

    public List<Teacher> GetAllTeachers()
    {
        return context.Teachers.ToList();
    }

    public Teacher GetTeacherByEmail(string email)
    {
        return context.Teachers.FirstOrDefault(t => t.Email == email);
    }

    public Teacher UpdateTeacher(Teacher teacher, string email)
    {
        var existingTeacher = GetTeacherByEmail(email);
        if (existingTeacher == null)
        {
            return null;
        }
        existingTeacher.FirstName = teacher.FirstName;
        existingTeacher.LastName = teacher.LastName;
        existingTeacher.Email = teacher.Email;
        context.SaveChanges();
        return existingTeacher;
    }

    public List<Course> GetCoursesOfTeacher(string email)
    {
        var teacher = context.Teachers.FirstOrDefault(t => t.Email == email);
        if (teacher == null)
        {
            return null;
        }
        return context.Courses
            .Where(c => c.TeacherId == teacher.Id)
            .ToList();
    }
}
