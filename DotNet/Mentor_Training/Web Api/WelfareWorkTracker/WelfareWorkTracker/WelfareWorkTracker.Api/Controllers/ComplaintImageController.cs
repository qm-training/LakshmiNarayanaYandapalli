namespace WelfareWorkTracker.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ComplaintImageController(IComplaintImageService complaintImageService) : ControllerBase
{
    private readonly IComplaintImageService _complaintImageService = complaintImageService;

    [HttpPost]
    public async Task<IActionResult> AddComplaintImage(ComplaintImageVm complaintImageVm)
    {
        var image = await _complaintImageService.AddComplaintImageAsync(complaintImageVm);
        if (image == null)
            return NotFound($"No such complaint exist with complaintId: {complaintImageVm.ComplaintId}");

        return Ok(new { message = "Image added successfully", image });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin/{id}")]
    public async Task<IActionResult> GetComplaintById(int id)
    {
        var complaintImage = await _complaintImageService.GetComplaintImageByIdAsync(id);
        if (complaintImage == null)
            return NotFound($"No ComplaintImage found with id: {id}");
        return Ok(complaintImage);
    }

    [HttpGet("citizen/{complaintId}")]
    public async Task<IActionResult> GetComplaintByComplaintId(int complaintId)
    {
        var complaintImage = await _complaintImageService.GetComplaintImageByIdAsync(complaintId);
        if (complaintImage == null)
            return NotFound($"No ComplaintImage found with id: {complaintId}");
        return Ok(complaintImage);
    }

    [HttpGet("allImages/{complaintId}")]
    public async Task<IActionResult> GetAllComplaintImagesByComplaintId(int complaintId)
    {
        var complaintImage = await _complaintImageService.GetComplaintImagesByComplaintIdAsync(complaintId);
        if (complaintImage == null)
            return NotFound($"No Images found with complaintId: {complaintId}");

        return Ok(complaintImage);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteComplaintImagesById(int complaintImageId)
    {
        var complaintImage = await _complaintImageService.DeleteComplaintImageByIdAsync(complaintImageId);
        if (!complaintImage)
            return NotFound("No image found");
        return Ok("Image deleted successfully");
    }
}
