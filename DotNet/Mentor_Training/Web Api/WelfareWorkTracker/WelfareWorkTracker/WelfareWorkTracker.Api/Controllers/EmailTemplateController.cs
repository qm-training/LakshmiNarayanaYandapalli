namespace WelfareWorkTracker.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmailTemplateController(IEmailTemplateService emailTemplateService) : ControllerBase
{
    private readonly IEmailTemplateService _emailTemplateService = emailTemplateService;

    /// <summary>
    /// Retrieves an email template by its unique identifier.
    /// </summary>
    /// <param name="templateId">The ID of the email template to retrieve.</param>
    /// <returns>
    /// Returns the email template if found; otherwise, returns an appropriate HTTP response.
    /// </returns>
    [Authorize]
    [HttpGet("{templateId}")]
    public async Task<IActionResult> GetTemplateById(int templateId)
    {
        var template = await _emailTemplateService.GetTemplateByIdAsync(templateId);
        return Ok(template);
    }

    /// <summary>
    /// Retrieves all available email templates.
    /// </summary>
    /// <returns>Returns a list of all email templates.</returns>
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllTemplates()
    {
        var templates = await _emailTemplateService.GetAllTemplatesAsync();
        return Ok(templates);
    }

    /// <summary>
    /// Retrieves an email template by its name.
    /// </summary>
    /// <param name="templateName">The name of the template to retrieve.</param>
    /// <returns>Returns the email template that matches the given name.</returns>
    [Authorize]
    [HttpGet("by-name")]
    public async Task<IActionResult> GetAllTemplatesByTemplateName([FromQuery] string templateName)
    {
        var templates = await _emailTemplateService.GetByNameAsync(templateName);
        return Ok(templates);
    }

    /// <summary>
    /// Creates a new email template.
    /// </summary>
    /// <param name="template">The email template information to be created.</param>
    /// <returns>
    /// Returns <c>201 Created</c> when the email template is successfully created.
    /// </returns>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> AddTemplate([FromBody] CreateEmailTemplateVm template)
    {
        var userId = User.GetUserIdAsInt();

        await _emailTemplateService.AddTemplateAsync(userId, template);
        return Created();
    }

    /// <summary>
    /// Updates an existing email template.
    /// </summary>
    /// <param name="templateId">The ID of the email template to update.</param>
    /// <param name="template">The updated template information.</param>
    /// <returns>
    /// Returns <c>NoContent</c> if the update is successful; otherwise, an appropriate HTTP response.
    /// </returns>
    [Authorize(Roles = "Admin")]
    [HttpPut("{templateId}")]
    public async Task<IActionResult> UpdateTemplate(int templateId, [FromBody] UpdateEmailTemplateVm template)
    {
        var userId = User.GetUserIdAsInt();

        await _emailTemplateService.UpdateTemplateAsync(templateId, userId, template);
        return NoContent();
    }

    /// <summary>
    /// Deletes (deactivates) an email template.
    /// </summary>
    /// <param name="templateId">The ID of the template to delete.</param>
    /// <returns>
    /// Returns <c>NoContent</c> if successfully deleted; otherwise, an appropriate HTTP response.
    /// </returns>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{templateId}")]
    public async Task<IActionResult> DeleteTemplate(int templateId)
    {
        var userId = User.GetUserIdAsInt();
        await _emailTemplateService.DeleteTemplateAsync(templateId, userId);
        return NoContent();
    }
}
