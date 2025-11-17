namespace WelfareWorkTracker.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ConstituencyController(IConstituencyService constituencyService) : ControllerBase
{
    private readonly IConstituencyService _constituencyService = constituencyService;

    /// <summary>
    /// Retrieves all constituencies available in the system.
    /// </summary>
    /// <returns>
    /// Returns a list of all constituencies.  
    /// If no constituencies exist, an exception is raised by the service layer.
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> GetConstituencies()
    {
        var constituencies = await _constituencyService.GetAllConstituenciesAsync();
        return Ok(constituencies);
    }

    /// <summary>
    /// Adds a new constituency to the system.
    /// </summary>
    /// <param name="constituencyVm">
    /// The constituency details including name, district, state, country, and pincode.
    /// </param>
    /// <returns>
    /// Returns the newly added constituency with its generated identifier.
    /// </returns>
    [HttpPost]
    public async Task<IActionResult> AddConstituency(ConstituencyVm constituencyVm)
    {
        var constituency = await _constituencyService.AddConstituencyAsync(constituencyVm);
        return Ok(constituency);
    }
}
