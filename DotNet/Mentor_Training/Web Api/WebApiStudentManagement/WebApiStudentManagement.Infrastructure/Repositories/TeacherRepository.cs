namespace WebApiStudentManagement.Infrastructure.Repositories;

public class TeacherRepository(WebApiStudentManagementDbContext context) : ITeacherRepository
{
    public async Task<string> AddTeacher(Teacher teacher)
    {
        await context.Teachers.AddAsync(teacher);
        await context.SaveChangesAsync();
        return "Teacher added successfully";
    }

    public async Task<string> DeleteTeacher(string email)
    {
        var teacher = await context.Teachers.FirstOrDefaultAsync(t => t.Email == email);
        if (teacher == null)
        {
            return "Teacher not found";
        }

        context.Teachers.Remove(teacher); // in-memory
        await context.SaveChangesAsync(); // async DB call
        return "Teacher deleted successfully";
    }

    public async Task<List<Teacher>> GetAllTeachers()
    {
        return await context.Teachers.ToListAsync();
    }

    public async Task<Teacher?> GetTeacherByEmail(string email)
    {
        return await context.Teachers.FirstOrDefaultAsync(t => t.Email == email);
    }

    public async Task<Teacher?> UpdateTeacher(Teacher teacher, string email)
    {
        var existingTeacher = await GetTeacherByEmail(email);
        if (existingTeacher == null)
        {
            return null;
        }

        existingTeacher.FirstName = teacher.FirstName;
        existingTeacher.LastName = teacher.LastName;
        existingTeacher.Email = teacher.Email;

        await context.SaveChangesAsync();
        return existingTeacher;
    }

    public async Task<List<Course>> GetCoursesOfTeacher(string email)
    {
        var teacher = await context.Teachers.FirstOrDefaultAsync(t => t.Email == email);
        if (teacher == null)
        {
            return new List<Course>();
        }

        return await context.Courses
            .Where(c => c.TeacherId == teacher.Id)
            .ToListAsync();
    }
}
