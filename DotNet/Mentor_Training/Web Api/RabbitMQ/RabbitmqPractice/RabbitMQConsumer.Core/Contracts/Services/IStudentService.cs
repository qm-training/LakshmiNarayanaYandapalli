namespace RabbitMQConsumer.Core.Contracts.Services;

public interface IStudentService
{
    Task<string> AddStudentAsync(StudentDto studentDto);
}
