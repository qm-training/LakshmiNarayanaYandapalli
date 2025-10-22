using AutoMapper;
using System.Reflection.Metadata.Ecma335;
using WelfareTracker.Core.Contracts.Repository;
using WelfareTracker.Core.Contracts.Service;
using WelfareTracker.Core.Dtos;
using WelfareTracker.Core.Enums;
using WelfareTracker.Core.Models;
using WelfareTracker.Core.Vms;

namespace WelfareTracker.Infrastructure.Service
{
    public class DailyComplaintService (IDailyComplaintRepository dailyComplaintRepository,
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
            foreach(var leader in leaders)
            {
                if(leader.Reputation < 15)
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
                    Status = (int) Status.Approve,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow
                };

                await _dailyComplaintStatusRepository.AddDailyComplaintStatusAsync(dailyComplaintStatus);
            }
        }

        public async Task<List<DailyComplaintDto>?> GetTodaysDailyComplaintsAsync()
        {
            var dailyComplaints = await _dailyComplaintRepository.GetTodaysDailyComplaintsAsync();
            if (dailyComplaints == null || dailyComplaints.Count == 0)
                return null;

            var dailyComplaintDtos = new List<DailyComplaintDto>();
            foreach(var dailyComplaint in dailyComplaints)
            {
                var dailyComplaintDto = _mapper.Map<DailyComplaintDto>(dailyComplaint);
                dailyComplaintDtos.Add(dailyComplaintDto);
            }
            return dailyComplaintDtos;
        }

        public async Task<DailyComplaintDto?> GetDailyComplaintByIdAsync(int dailyComplaintId)
        {
            var dailyComplaint = await _dailyComplaintRepository.GetDailyComplaintByIdAsync(dailyComplaintId);
            if(dailyComplaint == null)
                return null;

            var dailyComplaintDto = _mapper.Map<DailyComplaintDto>(dailyComplaint);
            return dailyComplaintDto;
        }

        public async Task<DailyComplaintDto?> GetDailyComplaintByLeaderIdAsync(int leaderId)
        {
            var dailyComplaint = await _dailyComplaintRepository.GetDailyComplaintForLeaderByLeaderIdAsync(leaderId);
            if (dailyComplaint == null)
                return null;

            var dailyComplaintDto = _mapper.Map<DailyComplaintDto>(dailyComplaint);
            return dailyComplaintDto;
        }

        public async Task<DailyComplaintDto?> UpdateDailyComplaintAsync(int dailyComplaintId, DailyComplaintVm dailyComplaintVm)
        {
            var dailyComplaint = await _dailyComplaintRepository.GetDailyComplaintByIdAsync(dailyComplaintId);
            if (dailyComplaint == null)
            {
                return null;
            }

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
