namespace RabbitMQConsumer.Infrastructure.Repository;

public class StudentRepository(RabbitMQContext _context) : IStudentRepository
{
    private readonly RabbitMQContext _context = _context;
    public async Task AddStudentAsync(Student student)
    {
        await _context.Students.AddAsync(student);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.InnerException?.Message);
        }

    }
}
