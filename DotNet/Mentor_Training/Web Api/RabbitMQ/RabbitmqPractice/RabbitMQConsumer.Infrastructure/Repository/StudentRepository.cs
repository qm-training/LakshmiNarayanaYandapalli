using RabbitMQConsumer.Core.Contracts.Repository;
using RabbitMQConsumer.Core.Models;
using RabbitMQConsumer.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQConsumer.Infrastructure.Repository
{
    public class StudentRepository(RabbitMQContext _context) : IStudentRepository
    {
        private readonly RabbitMQContext _context = _context;
        public async Task AddStudentAsync(Student student)
        {
            _context.Students.AddAsync(student);
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
}
