using RabbitMQConsumer.Core.Models;

namespace RabbitMQConsumer.Core.Contracts.Repository
{
    public interface IStudentRepository
    {
        Task AddStudentAsync(Student student);
    }
}
