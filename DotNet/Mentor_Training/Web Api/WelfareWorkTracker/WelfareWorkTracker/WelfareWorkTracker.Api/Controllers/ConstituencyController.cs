namespace WelfareWorkTracker.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ConstituencyController(IConstituencyService constituencyService) : ControllerBase
{
    private readonly IConstituencyService _constituencyService = constituencyService;

    [HttpGet]
    public async Task<IActionResult> GetConstituencies()
    {
        var constituencies = await _constituencyService.GetAllConstituenciesAsync();
        return Ok(constituencies);
    }

    [HttpPost]
    public async Task<IActionResult> AddConstituency(ConstituencyVm constituencyVm)
    {
        var constituency = await _constituencyService.AddConstituencyAsync(constituencyVm);
        return Ok(constituency);
    }
}
