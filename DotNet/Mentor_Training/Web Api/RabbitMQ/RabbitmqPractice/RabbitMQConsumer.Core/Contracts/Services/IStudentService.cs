using RabbitMQSender.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQConsumer.Core.Contracts.Services
{
    public interface IStudentService
    {
        Task<string> AddStudentAsync(StudentDto studentDto);
    }
}
