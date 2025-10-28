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
    public class DailyComplaintService(IDailyComplaintRepository dailyComplaintRepository,
                                            IDailyComplaintStatusRepository dailyComplaintStatusRepository,
                                            IUserRepository userRepository,
                                            IMapper mapper) : IDailyComplaintService
    {
        private readonly IDailyComplaintRepository _dailyComplaintRepository = dailyComplaintRepository;
        private readonly IDailyComplaintStatusRepository _dailyComplaintStatusRepository = dailyComplaintStatusRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IMapper _mapper = mapper;
        public async Task AssignDailyComplaintsToLeadersAsync()
        {
            var leaders = await _userRepository.GetAllLeadersAsync();
            if (leaders == null || leaders.Count == 0)
                throw new WelfareWorkTrackerException("No leaders found.", (int)HttpStatusCode.NotFound);

            foreach (var leader in leaders)
            {
                if (leader.Reputation < 15)
                {
                    continue;
                }
                var dailyComplaint = new DailyComplaint
                {
                    ConstituencyId = leader.ConstituencyId,
                    LeaderId = leader.UserId,
                    IsCompleted = false,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow
                };
                await _dailyComplaintRepository.AddDailyComplaintAsync(dailyComplaint);

                var dailyComplaintStatus = new DailyComplaintStatus
                {
                    DailyComplaintId = dailyComplaint.DailyComplaintId,
                    Status = (int)Status.Approve,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow
                };

                await _dailyComplaintStatusRepository.AddDailyComplaintStatusAsync(dailyComplaintStatus);
            }
        }

        public async Task<List<DailyComplaintDto>> GetTodaysDailyComplaintsAsync()
        {
            var dailyComplaints = await _dailyComplaintRepository.GetTodaysDailyComplaintsAsync();
            if (dailyComplaints == null || dailyComplaints.Count == 0)
                throw new WelfareWorkTrackerException("Daily complaint not found.", (int)HttpStatusCode.NotFound);

            var dailyComplaintDtos = new List<DailyComplaintDto>();
            foreach (var dailyComplaint in dailyComplaints)
            {
                var dailyComplaintDto = _mapper.Map<DailyComplaintDto>(dailyComplaint);
                dailyComplaintDtos.Add(dailyComplaintDto);
            }
            return dailyComplaintDtos;
        }

        public async Task<DailyComplaintDto> GetDailyComplaintByIdAsync(int dailyComplaintId)
        {
            var dailyComplaint = await _dailyComplaintRepository.GetDailyComplaintByIdAsync(dailyComplaintId);
            if (dailyComplaint == null)
                throw new WelfareWorkTrackerException("Daily complaint not found.", (int)HttpStatusCode.NotFound);

            var dailyComplaintDto = _mapper.Map<DailyComplaintDto>(dailyComplaint);
            return dailyComplaintDto;
        }

        public async Task<DailyComplaintDto> GetDailyComplaintByLeaderIdAsync(int leaderId)
        {
            var dailyComplaint = await _dailyComplaintRepository.GetDailyComplaintForLeaderByLeaderIdAsync(leaderId);
            if (dailyComplaint == null)
                throw new WelfareWorkTrackerException("Daily complaint not found.", (int)HttpStatusCode.NotFound);

            var dailyComplaintDto = _mapper.Map<DailyComplaintDto>(dailyComplaint);
            return dailyComplaintDto;
        }

        public async Task<DailyComplaintDto> UpdateDailyComplaintAsync(int dailyComplaintId, DailyComplaintVm dailyComplaintVm)
        {
            var dailyComplaint = await _dailyComplaintRepository.GetDailyComplaintByIdAsync(dailyComplaintId);
            if (dailyComplaint == null)
                throw new WelfareWorkTrackerException("Daily complaint not found.", (int)HttpStatusCode.NotFound);

            dailyComplaint.ConstituencyId = dailyComplaintVm.ConstituencyId;
            dailyComplaint.LeaderId = dailyComplaintVm.LeaderId;
            dailyComplaint.IsCompleted = dailyComplaintVm.IsCompleted;
            dailyComplaint.DateUpdated = DateTime.UtcNow;

            await _dailyComplaintRepository.UpdateDailyComplaintAsync(dailyComplaint);

            var dailyComplaintDto = _mapper.Map<DailyComplaintDto>(dailyComplaint);
            return dailyComplaintDto;
        }
    }
}
