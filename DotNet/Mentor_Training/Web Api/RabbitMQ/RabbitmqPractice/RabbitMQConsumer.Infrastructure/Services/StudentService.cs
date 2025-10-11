using RabbitMQConsumer.Core.Contracts.Repository;
using RabbitMQConsumer.Core.Contracts.Services;
using RabbitMQConsumer.Core.Models;
using RabbitMQSender.Core.Dtos;

namespace RabbitMQConsumer.Infrastructure.Services
{
    public class StudentService(IStudentRepository repository) : IStudentService
    {
        private readonly IStudentRepository _repository = repository;
        public async Task<string> AddStudentAsync(StudentDto studentDto)
        {
            Student student = new Student
            {
                Name = studentDto.Name,
                Age = studentDto.Age,
                Address = studentDto.Address
            };

            await _repository.AddStudentAsync(student);
            return "Student added successfully";
        }
    }
}
