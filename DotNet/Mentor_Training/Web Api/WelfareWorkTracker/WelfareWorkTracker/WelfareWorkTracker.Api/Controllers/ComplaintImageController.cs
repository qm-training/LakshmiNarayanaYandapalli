namespace WelfareWorkTracker.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ComplaintImageController(IComplaintImageService complaintImageService) : ControllerBase
{
    private readonly IComplaintImageService _complaintImageService = complaintImageService;

    /// <summary>
    /// Adds an image related to a specific complaint.
    /// </summary>
    /// <param name="complaintImageVm">
    /// The complaint image details including ComplaintId and image URL(s).
    /// </param>
    /// <returns>
    /// Returns a success message with added image details if successful; 
    /// otherwise, returns a not found result if the complaint does not exist.
    /// </returns>
    [HttpPost]
    public async Task<IActionResult> AddComplaintImage(ComplaintImageVm complaintImageVm)
    {
        var image = await _complaintImageService.AddComplaintImageAsync(complaintImageVm);
        if (image == null)
            return NotFound($"No such complaint exist with complaintId: {complaintImageVm.ComplaintId}");

        return Ok(new { message = "Image added successfully", image });
    }

    /// <summary>
    /// Retrieves a complaint image by its unique ID.  
    /// Accessible only by Admin users.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the complaint image.
    /// </param>
    /// <returns>
    /// Returns the complaint image if found; otherwise, a not found result.
    /// </returns>
    [Authorize(Roles = "Admin")]
    [HttpGet("admin/{id}")]
    public async Task<IActionResult> GetComplaintById(int id)
    {
        var complaintImage = await _complaintImageService.GetComplaintImageByIdAsync(id);
        if (complaintImage == null)
            return NotFound($"No ComplaintImage found with id: {id}");
        return Ok(complaintImage);
    }

    /// <summary>
    /// Retrieves a complaint image (citizen view) using the complaint ID.
    /// </summary>
    /// <param name="complaintId">
    /// The complaint ID for which the related image is requested.
    /// </param>
    /// <returns>
    /// Returns the complaint image if found; otherwise, a not found result.
    /// </returns>
    [HttpGet("citizen/{complaintId}")]
    public async Task<IActionResult> GetComplaintByComplaintId(int complaintId)
    {
        var complaintImage = await _complaintImageService.GetComplaintImageByIdAsync(complaintId);
        if (complaintImage == null)
            return NotFound($"No ComplaintImage found with id: {complaintId}");
        return Ok(complaintImage);
    }

    /// <summary>
    /// Retrieves all images associated with a specific complaint.
    /// </summary>
    /// <param name="complaintId">
    /// The ID of the complaint for which all images should be retrieved.
    /// </param>
    /// <returns>
    /// Returns a list of complaint images if available; otherwise, a not found result.
    /// </returns>
    [HttpGet("allImages/{complaintId}")]
    public async Task<IActionResult> GetAllComplaintImagesByComplaintId(int complaintId)
    {
        var complaintImage = await _complaintImageService.GetComplaintImagesByComplaintIdAsync(complaintId);
        if (complaintImage == null)
            return NotFound($"No Images found with complaintId: {complaintId}");

        return Ok(complaintImage);
    }

    /// <summary>
    /// Deletes a complaint image by its unique image ID.
    /// </summary>
    /// <param name="complaintImageId">
    /// The ID of the image to be deleted.
    /// </param>
    /// <returns>
    /// Returns a success message if deletion is successful; 
    /// otherwise, returns a not found result.
    /// </returns>
    [HttpDelete]
    public async Task<IActionResult> DeleteComplaintImagesById(int complaintImageId)
    {
        var complaintImage = await _complaintImageService.DeleteComplaintImageByIdAsync(complaintImageId);
        if (!complaintImage)
            return NotFound("No image found");
        return Ok("Image deleted successfully");
    }
}
