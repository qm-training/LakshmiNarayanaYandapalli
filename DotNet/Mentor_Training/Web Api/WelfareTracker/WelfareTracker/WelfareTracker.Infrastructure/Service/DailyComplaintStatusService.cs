using AutoMapper;
using System.Net;
using WelfareTracker.Core.Contracts.Repository;
using WelfareTracker.Core.Contracts.Service;
using WelfareTracker.Core.Dtos;
using WelfareTracker.Core.Enums;
using WelfareTracker.Core.Exceptions;
using WelfareTracker.Core.Models;
using WelfareTracker.Core.Vms;

namespace WelfareTracker.Infrastructure.Service
{
    public class DailyComplaintStatusService (IClaimsService claimsService,
                                                IUserRepository userRepository,
                                                IDailyComplaintRepository dailyComplaintRepository,
                                                IDailyComplaintStatusRepository dailyComplaintStatusRepository,
                                                IMapper mapper) : IDailyComplaintStatusService
    {
        private readonly IClaimsService _claimsService = claimsService;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IDailyComplaintRepository _dailyComplaintRepository = dailyComplaintRepository;
        private readonly IDailyComplaintStatusRepository _dailyComplaintStatusRepository = dailyComplaintStatusRepository;
        private readonly IMapper _mapper = mapper;
        public async Task<DailyComplaintStatusDto> AddDailyComplaintStatusByLeader(DailyComplaintStatusVm dailyComplaintStatusVm)
        {
            var userId = await _claimsService.GetUserIdFromClaimsAsync();
            var user = await _userRepository.GetUserByIdAsync(userId) ?? throw new WelfareTrackerException("User not found");

            var dailyComplaint = await _dailyComplaintRepository.GetDailyComplaintByIdAsync(dailyComplaintStatusVm.DailyComplaintId) ?? 
                                                                throw new WelfareTrackerException("Daily complaint not found");
            var dailyComplaintStatus = await _dailyComplaintStatusRepository.GetDailyComplaintStatusByDailyComplaintIdAsync(dailyComplaintStatusVm.DailyComplaintId) ??
                                                                throw new WelfareTrackerException("Daily complaint status not found");

            if (user.ConstituencyId != dailyComplaint.ConstituencyId)
                throw new WelfareTrackerException("Cannot update daily complaint status", (int)HttpStatusCode.Unauthorized);

            if (dailyComplaintStatusVm.Status == (int)Status.InProgress) // InProgress
            {
                // Checking the current status is 'Approved' (6) before making updates
                if (dailyComplaintStatus.Status != (int)Status.Approve)
                {
                    throw new WelfareTrackerException("The complaint must be in 'Approve' (6) status to be updated to InProgress.");
                }
                else if (dailyComplaintStatusVm.Status == (int)Status.Resolved)
                {
                    // Checking the current status is 'InProgress' (6) before making updates
                    if (dailyComplaintStatus.Status != (int)Status.InProgress)
                        throw new WelfareTrackerException("The complaint must be in 'InProgress' (7) status to be updated to Resolved.");
                    else
                        throw new ArgumentException("Invalid status provided.");
                }
            }

            var newDailyComplaintStatus = new DailyComplaintStatus
            {
                DailyComplaintId = dailyComplaintStatus.DailyComplaintId,
                Status = dailyComplaintStatusVm.Status,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now
            };

            var updatedDailyComplaintStatus = await _dailyComplaintStatusRepository.AddDailyComplaintStatusAsync(newDailyComplaintStatus);

            var dailyComplaintStatusDto = _mapper.Map<DailyComplaintStatusDto>(updatedDailyComplaintStatus);
            return dailyComplaintStatusDto;
        }
    }
}
