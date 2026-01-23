using WelfareWorkTracker.Core.Models;

namespace WelfareWorkTracker.Infrastructure.Service;
public class DailyComplaintStatusService(IDailyComplaintStatusRepository dailyComplaintStatusRepository,
                                            IUserRepository userRepository,
                                            IDailyComplaintService dailyComplaintService,
                                            IEmailTemplateRepository emailTemplateRepository,
                                            IEmailService emailService,
                                            IMapper mapper) : IDailyComplaintStatusService
{
    private readonly IDailyComplaintStatusRepository _dailyComplaintStatusRepository = dailyComplaintStatusRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IDailyComplaintService _dailyComplaintService = dailyComplaintService;
    private readonly IEmailTemplateRepository _emailTemplateRepository = emailTemplateRepository;
    private readonly IEmailService _emailService = emailService;
    private readonly IMapper _mapper = mapper;

    public async Task<DailyComplaintStatusDto?> UpdateDailyComplaintStatusByLeaderAsync(DailyComplaintStatusVm dailyComplaintStatusVm)
    {
        // Fetching the current complaint status from the repository
        var dailyComplaintStatus = await _dailyComplaintStatusRepository.GetDailyComplaintStatusAsync(dailyComplaintStatusVm.DailyComplaintId);

        // If no complaint status is found, return null
        if (dailyComplaintStatus == null)
            return null;

        if (dailyComplaintStatusVm.Status == (int)Status.InProgress) // InProgress
        {
            // Checking the current status is 'Approved' (6) before making updates
            if (dailyComplaintStatus.Status != (int)Status.Approve)
            {
                throw new WelfareWorkTrackerException("The complaint must be in 'Approve' (6) status to be updated to InProgress.");
            }
            else if (dailyComplaintStatusVm.Status == (int)Status.Resolved)
            {
                // Checking the current status is 'InProgress' (6) before making updates
                if (dailyComplaintStatus.Status != (int)Status.InProgress)
                    throw new WelfareWorkTrackerException("The complaint must be in 'InProgress' (7) status to be updated to Resolved.");
                else
                    throw new ArgumentException("Invalid status provided.");
            }
        }

        var newStatusRecord = new DailyComplaintStatus
        {
            DailyComplaintId = dailyComplaintStatus.DailyComplaintId,
            Status = dailyComplaintStatusVm.Status,
            DateCreated = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow,
        };

        await _dailyComplaintStatusRepository.AddDailyComplaintStatusAsync(newStatusRecord);

        return _mapper.Map<DailyComplaintStatusDto>(newStatusRecord);
    }

    public async Task CheckDailyComplaintStatusAsync()
    {
        var dailyComplaintStatus = await _dailyComplaintStatusRepository.GetPendingDailyComplaintsAsync();

        foreach (var complaintStatus in dailyComplaintStatus)
        {
            var dailyComplaint = await _dailyComplaintService.GetDailyComplaintByIdAsync(complaintStatus!.DailyComplaintId);

            var leader = await _userRepository.GetUserByIdAsync(dailyComplaint!.LeaderId);

            if (leader!.Reputation <= 15)
            {
                // Skip further processing for this leader
                continue;
            }

            var newReputation = await DecreaseLeaderReputationAsync(leader);

            // adding status record with status Unresolved
            var newStatusRecord = new DailyComplaintStatus
            {
                DailyComplaintId = dailyComplaint.DailyComplaintId,
                Status = (int)Status.Unresolved,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
            };
            await _dailyComplaintStatusRepository.AddDailyComplaintStatusAsync(newStatusRecord);

            var emailTemplate = await _emailTemplateRepository.GetByNameAsync(Constants.EmailTemplates.DailyComplaintLeaderUnresolved)
                                    ?? throw new WelfareWorkTrackerException($"Email Template Not Found!", (int)HttpStatusCode.NotFound);
            var payload = new Dictionary<string, string> {
                {"LeaderName", leader.FullName },
                {"PreviousReputation", leader.Reputation.ToString() },
                { "NewReputation", newReputation.ToString() }
            };
            var email = new EmailVm
            {
                ToUserEmail = leader.Email,
                TemplateId = emailTemplate.Id,
                Payload = payload
            };
            await _emailService.SendEmailAsync(email);
        }
    }

    private async Task<double> DecreaseLeaderReputationAsync(User leader)
    {
        leader.Reputation -= 2;
        if (leader.Reputation < 0)
            leader.Reputation = 0;

        await _userRepository.UpdateLeaderReputationAsync(leader.UserId, leader.Reputation);

        if (leader.Reputation < 15)
        {
            //await TriggerReElectionAsync(leader)
        }

        return leader.Reputation;
    }
}
