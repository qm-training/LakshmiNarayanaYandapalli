namespace WelfareWorkTracker.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ComplaintImageController(IComplaintImageService complaintImageService) : ControllerBase
{
    private readonly IComplaintImageService _complaintImageService = complaintImageService;

    /// <summary>
    /// Adds an image for a specific complaint submitted by a citizen.
    /// </summary>
    /// <param name="complaintImageVm">The vm containing the image data and associated complaint information.</param>
    /// <returns>Returns a success message and the added image if the operation is successful; otherwise, returns a not found result.</returns>
    [HttpPost]
    public async Task<IActionResult> AddComplaintImage(ComplaintImageVm complaintImageVm)
    {
        var image = await _complaintImageService.AddComplaintImageAsync(complaintImageVm);
        if (image == null)
            return NotFound($"No such complaint exist with complaintId: {complaintImageVm.ComplaintId}");

        return Ok(new { message = "Image added successfully", image });
    }

    /// <summary>
    /// To get a image/record from a db - Admin
    /// </summary>
    /// <param name="id">The unique identifier of the complaint image to retrieve.</param>
    /// <returns>Returns the complaint image if found; otherwise, returns a not found result.</returns>
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
    /// To get a complaint images related to a complaint - citizen
    /// </summary>
    /// <param name="complaintId">The unique identifier of the complaint for which images are being retrieved.</param>
    /// <returns>Returns the complaint image if found, or a NotFound result if no image exist for the provided complaint ID.</returns>
    [HttpGet("citizen/{complaintId}")]
    public async Task<IActionResult> GetComplaintByComplaintId(int complaintId)
    {
        var complaintImage = await _complaintImageService.GetComplaintImageByIdAsync(complaintId);
        if (complaintImage == null)
            return NotFound($"No ComplaintImage found with id: {complaintId}");
        return Ok(complaintImage);
    }

    /// <summary>
    /// To retreive all images related to a specific complaint
    /// </summary>
    /// <param name="complaintId">The unique identifier of the complaint for which images are being retrieved.</param>
    /// <returns>Returns the complaint images if found, or a NotFound result if no images exist for the provided complaint ID.</returns>
    [HttpGet("allImages/{complaintId}")]
    public async Task<IActionResult> GetAllComplaintImagesByComplaintId(int complaintId)
    {
        var complaintImage = await _complaintImageService.GetComplaintImagesByComplaintIdAsync(complaintId);
        if (complaintImage == null)
            return NotFound($"No Images found with complaintId: {complaintId}");

        return Ok(complaintImage);
    }

    /// <summary>
    /// To delete an image
    /// </summary>
    /// <param name="complaintImageId">The unique identifier of the complaint image to be deleted.</param>
    /// <returns>Returns a success message if the image is deleted, or a NotFound result if no image exists with the provided ID.</returns>
    [HttpDelete]
    public async Task<IActionResult> DeleteComplaintImagesById(int complaintImageId)
    {
        var complaintImage = await _complaintImageService.DeleteComplaintImageByIdAsync(complaintImageId);
        if (!complaintImage)
            return NotFound("No image found");
        return Ok("Image deleted successfully");
    }
}
