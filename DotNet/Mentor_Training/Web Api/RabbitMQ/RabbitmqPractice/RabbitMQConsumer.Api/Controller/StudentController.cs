using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RabbitMQConsumer.Core.Contracts.Services;
using RabbitMQSender.Core.Dtos;

namespace RabbitMQConsumer.Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController(IStudentService service) : ControllerBase
    {
        private readonly IStudentService _service = service;

        [HttpPost]
        public async Task<IActionResult> AddStudentAsync([FromBody]StudentDto studentDto)
        {

            await _service.AddStudentAsync(studentDto);

            return Ok("Student added successfully");

        }
    }
}
