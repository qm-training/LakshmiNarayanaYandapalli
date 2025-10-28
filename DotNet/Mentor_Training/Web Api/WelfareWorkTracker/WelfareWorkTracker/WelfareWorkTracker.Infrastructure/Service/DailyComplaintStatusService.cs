using AutoMapper;
using System.Net;
using WelfareWorkTracker.Core.Contracts.Repository;
using WelfareWorkTracker.Core.Contracts.Service;
using WelfareWorkTracker.Core.Dtos;
using WelfareWorkTracker.Core.Enums;
using WelfareWorkTracker.Core.Exceptions;
using WelfareWorkTracker.Core.Models;
using WelfareWorkTracker.Core.Vms;

namespace WelfareWorkTracker.Infrastructure.Service
{
    public class DailyComplaintStatusService(IClaimsService claimsService,
                                                IUserRepository userRepository,
                                                IDailyComplaintRepository dailyComplaintRepository,
                                                IDailyComplaintStatusRepository dailyComplaintStatusRepository,
                                                IDailyComplaintService dailyComplaintService,
                                                IMapper mapper) : IDailyComplaintStatusService
    {
        private readonly IClaimsService _claimsService = claimsService;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IDailyComplaintRepository _dailyComplaintRepository = dailyComplaintRepository;
        private readonly IDailyComplaintStatusRepository _dailyComplaintStatusRepository = dailyComplaintStatusRepository;
        private readonly IDailyComplaintService _dailyComplaintService = dailyComplaintService;
        private readonly IMapper _mapper = mapper;
        public async Task<DailyComplaintStatusDto> AddDailyComplaintStatusByLeader(DailyComplaintStatusVm dailyComplaintStatusVm)
        {
            var userId = await _claimsService.GetUserIdFromClaimsAsync();
            var user = await _userRepository.GetUserByIdAsync(userId) 
                                ?? throw new WelfareWorkTrackerException("User not found", (int)HttpStatusCode.NotFound);

            var dailyComplaint = await _dailyComplaintRepository.GetDailyComplaintByIdAsync(dailyComplaintStatusVm.DailyComplaintId)
                                            ?? throw new WelfareWorkTrackerException("Daily complaint not found", (int)HttpStatusCode.NotFound);
            var dailyComplaintStatus = await _dailyComplaintStatusRepository.GetDailyComplaintStatusByDailyComplaintIdAsync(dailyComplaintStatusVm.DailyComplaintId)
                                            ?? throw new WelfareWorkTrackerException("Daily complaint status not found", (int)HttpStatusCode.NotFound);

            if (user.ConstituencyId != dailyComplaint.ConstituencyId)
                throw new WelfareWorkTrackerException("Cannot update daily complaint status", (int)HttpStatusCode.Unauthorized);

            if (dailyComplaintStatusVm.Status == (int)Status.InProgress)
            {
                if (dailyComplaintStatus.Status != (int)Status.Approve)
                {
                    throw new WelfareWorkTrackerException("The complaint must be in Approve status to be updated to InProgress.");
                }
                else if (dailyComplaintStatusVm.Status == (int)Status.Resolved)
                {
                    if (dailyComplaintStatus.Status != (int)Status.InProgress)
                        throw new WelfareWorkTrackerException("The complaint must be in InProgress status to be updated to Resolved.");
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

        public async Task CheckDailyComplaintStatusAsync()
        {
            var dailyComplaintStatus = await _dailyComplaintStatusRepository.GetPendingDailyComplaintsAsync();

            foreach (var complaintStatus in dailyComplaintStatus)
            {
                var dailyComplaint = await _dailyComplaintService.GetDailyComplaintByIdAsync(complaintStatus!.DailyComplaintId);

                var leader = await _userRepository.GetUserByIdAsync(dailyComplaint!.LeaderId)
                                                    ?? throw new WelfareWorkTrackerException("Leader not found.", (int)HttpStatusCode.NotFound);

                if (leader.Reputation <= 15)
                {
                    continue;
                }

                await DecreaseLeaderReputationAsync(leader);

                var newStatusRecord = new DailyComplaintStatus
                {
                    DailyComplaintId = dailyComplaint.DailyComplaintId,
                    Status = (int)Status.Unresolved,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,
                };
                await _dailyComplaintStatusRepository.AddDailyComplaintStatusAsync(newStatusRecord);
            }
        }

        private async Task DecreaseLeaderReputationAsync(User leader)
        {
            leader.Reputation -= 2;
            if (leader.Reputation < 0)
                leader.Reputation = 0;

            await _userRepository.UpdateLeaderReputationAsync(leader.UserId, leader.Reputation);

            if (leader.Reputation < 15)
            {
                //await TriggerReElectionAsync(leader)
            }
        }
    }
}
