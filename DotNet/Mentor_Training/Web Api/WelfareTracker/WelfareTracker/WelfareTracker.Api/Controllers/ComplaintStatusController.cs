using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WelfareTracker.Core.Contracts.Service;
using WelfareTracker.Core.Vms;

namespace WelfareTracker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComplaintStatusController(IComplaintStatusService complaintStatusService) : ControllerBase
    {
        private readonly IComplaintStatusService _complaintStatusService = complaintStatusService;

        [HttpPost("validateByAdminRep")]
        public async Task<IActionResult> ValidateComplaintStatusByAdminRep(AdminRepStatusVm adminRepStatusVm)
        {
            var result = await _complaintStatusService.AddComplaintStatusByAdminRep(adminRepStatusVm);
            if(result == null)
            {
                return BadRequest("Unable to add complaint status.");
            }
            return Ok(result);
        }
    }
}
