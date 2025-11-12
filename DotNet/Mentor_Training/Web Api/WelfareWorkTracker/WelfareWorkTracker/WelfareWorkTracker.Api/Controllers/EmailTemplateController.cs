namespace WelfareWorkTracker.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmailTemplateController(IEmailTemplateService emailTemplateService) : ControllerBase
{
    private readonly IEmailTemplateService _emailTemplateService = emailTemplateService;

    [Authorize]
    [HttpGet("{templateId}")]
    public async Task<IActionResult> GetTemplateById(int templateId)
    {
        var template = await _emailTemplateService.GetTemplateByIdAsync(templateId);
        return Ok(template);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllTemplates()
    {
        var templates = await _emailTemplateService.GetAllTemplatesAsync();
        return Ok(templates);
    }

    [Authorize]
    [HttpGet("by-name")]
    public async Task<IActionResult> GetAllTemplatesByTemplateName([FromQuery] string templateName)
    {
        var templates = await _emailTemplateService.GetByNameAsync(templateName);
        return Ok(templates);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> AddTemplate([FromBody] CreateEmailTemplateVm template)
    {
        var userId = User.GetUserIdAsInt();

        await _emailTemplateService.AddTemplateAsync(userId, template);
        return Created();
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{templateId}")]
    public async Task<IActionResult> UpdateTemplate(int templateId, [FromBody] UpdateEmailTemplateVm template)
    {
        var userId = User.GetUserIdAsInt();

        await _emailTemplateService.UpdateTemplateAsync(templateId, userId, template);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{templateId}")]
    public async Task<IActionResult> DeleteTemplate(int templateId)
    {
        var userId = User.GetUserIdAsInt();
        await _emailTemplateService.DeleteTemplateAsync(templateId, userId);
        return NoContent();
    }
}
