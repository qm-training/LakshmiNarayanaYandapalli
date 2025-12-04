namespace RabbitMQSender.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RabbitMQController(RabbitMQServices rabbitMQServices) : ControllerBase
{
    private readonly RabbitMQServices _rabbitMQServices = rabbitMQServices;

    [HttpPost("publish")]
    public async Task<IActionResult> PublishMessage([FromBody] StudentDto message)
    {
        if (message == null)
        {
            return BadRequest("Message cannot be null.");
        }
        await _rabbitMQServices.PublishAsync(message);
        return Ok("Message published successfully.");
    }
}
