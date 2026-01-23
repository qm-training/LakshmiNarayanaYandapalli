namespace WelfareWorkTracker.Infrastructure.Service;
public class DailyComplaintService(IDailyComplaintRepository dailyComplaintRepository,
                                    IUserRepository userRepository,
                                    IDailyComplaintStatusRepository dailyComplaintStatusRepository,
                                    IMapper mapper) : IDailyComplaintService
{
    private readonly IDailyComplaintRepository _dailyComplaintRepository = dailyComplaintRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IDailyComplaintStatusRepository _dailyComplaintStatusRepository = dailyComplaintStatusRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<DailyComplaintDto?> GetDailyComplaintByIdAsync(int id)
    {
        var dailyComplaint = await _dailyComplaintRepository.GetDailyComplaintByIdAsync(id);
        if (dailyComplaint == null)
            return null;

        return _mapper.Map<DailyComplaintDto>(dailyComplaint);
    }

    public async Task<DailyComplaintDto?> GetDailyComplaintByLeaderIdAsync(int leaderId)
    {
        var dailyComplaint = await _dailyComplaintRepository.GetDailyComplaintByLeaderIdAsync(leaderId);
        if (dailyComplaint == null)
            return null;

        return _mapper.Map<DailyComplaintDto>(dailyComplaint);
    }

    public async Task<List<DailyComplaintDto>> GetDailyComplaintsAsync()
    {
        var dailyComplaints = await _dailyComplaintRepository.GetDailyComplaintsAsync();

        return _mapper.Map<List<DailyComplaintDto>>(dailyComplaints);
    }

    public async Task<DailyComplaintDto?> UpdateDailyComplaintAsync(int id, DailyComplaintVm dailyComplaintVm)
    {
        var dailyComplaint = await _dailyComplaintRepository.GetDailyComplaintByIdAsync(id);
        if (dailyComplaint == null)
            return null;
        var leader = await _userRepository.GetLeaderByConstituencyIdAsync(dailyComplaint.ConstituencyId);

        dailyComplaint.ConstituencyId = dailyComplaintVm.ConstituencyId;
        dailyComplaint.IsCompleted = dailyComplaintVm.IsCompleted;
        dailyComplaint.LeaderId = leader!.UserId;
        dailyComplaint.DateUpdated = DateTime.UtcNow;
        var updatedDailyComplaint = await _dailyComplaintRepository.UpdateDailyComplaintAsync(dailyComplaint);

        return _mapper.Map<DailyComplaintDto>(updatedDailyComplaint);
    }

    public async Task AssignDailyComplaintsAsync()
    {
        // Fetching leaders and assigning a daily complaint to each one
        var leaders = await _userRepository.GetAllLeadersAsync();

        foreach (var leader in leaders!)
        {
            if (leader.Reputation <= 15)
            {
                // Stop further processing for this leader
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

            // Save the daily complaint to the database
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
    public async Task<DailyComplaintDto?> GetDailyComplaintByConstituencyNameAsync(string constituencyName)
    {
        var dailyComplaint = await _dailyComplaintRepository.GetDailyComplaintsByConstituencyNameAsync(constituencyName);
        if (dailyComplaint == null)
            return null;

        return _mapper.Map<DailyComplaintDto>(dailyComplaint);
    }
}
