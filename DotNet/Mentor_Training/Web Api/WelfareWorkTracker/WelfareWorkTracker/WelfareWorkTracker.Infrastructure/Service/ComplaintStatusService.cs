
namespace WelfareWorkTracker.Infrastructure.Service;
public class ComplaintStatusService(IComplaintStatusRepository complaintStatusRepository,
                                        IComplaintRepository complaintRepository,
                                        IUserRepository userRepository,
                                        IConstituencyRepository constituencyRepository,
                                        IClaimsService claimsService,
                                        IMapper mapper,
                                        IEmailTemplateRepository emailTemplateRepository,
                                        IEmailService emailService) : IComplaintStatusService
{
    private readonly IComplaintStatusRepository _complaintStatusRepository = complaintStatusRepository;
    private readonly IComplaintRepository _complaintRepository = complaintRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IConstituencyRepository _constituencyRepository = constituencyRepository;
    private readonly IClaimsService _claimsService = claimsService;
    private readonly IMapper _mapper = mapper;
    private readonly IEmailTemplateRepository _emailTemplateRepository = emailTemplateRepository;
    private readonly IEmailService _emailService = emailService;
    public async Task<ComplaintStatusDto?> AddComplaintStatusByAdminAsync(StatusByAdminVm statusByAdminVm)
    {
        var userId = await _claimsService.GetUserIdFromClaimsAsync();
        var user = await _userRepository.GetUserByIdAsync(userId);
        var complaint = await _complaintRepository.GetComplaintByComplaintIdAsync(statusByAdminVm.ComplaintId)
            ?? throw new WelfareWorkTrackerException($"No complaint found with complaintId: {statusByAdminVm.ComplaintId}");

        var leader = await _userRepository.GetLeaderByConstituencyIdAsync(complaint.ConstituencyId);

        if (leader!.Reputation <= 15)
        {
            // Stop further processing for this leader
            throw new WelfareWorkTrackerException("Leader is not eligible");
        }
        var constituencyName = await _constituencyRepository.GetConstituencyNameByIdAsync(user!.ConstituencyId);
        var complaints = await _complaintRepository.GetComplaintsByConstituency(constituencyName!);

        // Checking if the complaint exists in the list of complaints for this admin constituency
        if (!complaints.Any(c => c.ComplaintId == statusByAdminVm.ComplaintId))
        {
            throw new WelfareWorkTrackerException("You can validate only complaints from your constituency.");
        }

        // Fetching the current complaint status from the repository
        var complaintStatus = await _complaintStatusRepository.GetComplaintStatusAsync(statusByAdminVm.ComplaintId);

        // If no complaint status is found, return null
        if (complaintStatus == null)
            return null;

        // Checking if the current status is already updated
        if (complaintStatus.Status != (int)Status.Valid && complaintStatus.Status != (int)Status.Invalid)
            throw new WelfareWorkTrackerException("You cannot update");  // Status is already updated by admin

        if (statusByAdminVm.Status != (int)Status.Backlog && statusByAdminVm.Status != (int)Status.Reject)
            throw new WelfareWorkTrackerException("Invalid status provided");

        // Determining the new status based on the admin representative's input
        int newStatus;
        ComplaintStatus newStatusRecord;
        if (complaintStatus.Status == (int)Status.Valid && statusByAdminVm.Status == (int)Status.Backlog)
        {
            newStatus = 4; // Status for "backlog"

            newStatusRecord = new ComplaintStatus
            {
                ComplaintId = complaintStatus.ComplaintId,
                AttemptNumber = complaintStatus.AttemptNumber,
                Status = newStatus,
                OpenedDate = DateTime.UtcNow,     // when Admin updates the status, opened date will be created
                DateUpdated = DateTime.UtcNow,
                DateCreated = complaintStatus.DateCreated,
            };
        }
        else if (complaintStatus.Status == (int)Status.Invalid && statusByAdminVm.Status == (int)Status.Reject)
        {
            newStatus = 5; // Status for "rejected"

            newStatusRecord = new ComplaintStatus
            {
                ComplaintId = complaintStatus.ComplaintId,
                AttemptNumber = complaintStatus.AttemptNumber,
                Status = newStatus,
                DateUpdated = DateTime.Now,
                DateCreated = complaintStatus.DateCreated,
            };
        }
        else
        {
            throw new ArgumentException("Invalid status provided");
        }

        // Adding the new status record to the repository
        await _complaintStatusRepository.AddComplaintStatusAsync(newStatusRecord);


        var citizen = await _userRepository.GetUserByIdAsync(complaint.CitizenId);

        // notify leader and citizen if it is backlogged 
        if (newStatusRecord.Status == (int)Status.Backlog)
        {
            var leaderBacklogTemplate = await _emailTemplateRepository.GetByNameAsync(Constants.EmailTemplates.ComplaintLeaderBacklog)
                    ?? throw new WelfareWorkTrackerException($"Email Template Not Found!", (int)HttpStatusCode.NotFound);
            var citizenBacklogTemplate = await _emailTemplateRepository.GetByNameAsync(Constants.EmailTemplates.ComplaintCitizenBacklog)
                    ?? throw new WelfareWorkTrackerException($"Email Template Not Found!", (int)HttpStatusCode.NotFound);

            var leaderBacklogPayload = new Dictionary<string, string> {
                        { "LeaderName",leader.FullName},
                        { "ComplaintTitle",complaint.Title}
                    };

            var citizenBacklogPayload = new Dictionary<string, string> {
                        { "CitizenName",citizen!.FullName},
                        { "ComplaintTitle",complaint.Title}
                    };


            var citizenEmailVm = new EmailVm
            {
                ToUserEmail = citizen.Email,
                TemplateId = citizenBacklogTemplate.Id,
                Payload = citizenBacklogPayload
            };

            var leaderEmailVm = new EmailVm
            {
                ToUserEmail = leader.Email,
                TemplateId = leaderBacklogTemplate.Id,
                Payload = leaderBacklogPayload
            };

            await _emailService.SendEmailAsync(citizenEmailVm);
            await _emailService.SendEmailAsync(leaderEmailVm);

        }
        // notify citizen if it is rejected by admin
        else
        {
            var citizenRejectedTemplate = await _emailTemplateRepository.GetByNameAsync(Constants.EmailTemplates.ComplaintCitizenRejected) 
                            ?? throw new WelfareWorkTrackerException($"Email Template Not Found!", (int)HttpStatusCode.NotFound);

            var reason = await _complaintStatusRepository.GetComplaintStatusAsync(statusByAdminVm.ComplaintId) 
                ?? throw new WelfareWorkTrackerException("Complaint status not found!");

            var citizenRejectPayload = new Dictionary<string, string> {
                        { "CitizenName" , citizen!.FullName},
                        { "ComplaintTitle", complaint.Title},
                        { "RejectReason", reason.RejectReason ?? "Unknown Reason!"}
                    };

            var citizenRejectEmailVm = new EmailVm
            {
                ToUserEmail = citizen.Email,
                TemplateId = citizenRejectedTemplate.Id,
                Payload = citizenRejectPayload
            };
            await _emailService.SendEmailAsync(citizenRejectEmailVm);
        }

        // Returning the updated complaint status as a DTO
        var complaintStatusDto = _mapper.Map<ComplaintStatusDto>(newStatusRecord);
        return complaintStatusDto;
    }

    public async Task<ComplaintStatusDto?> AddComplaintStatusByAdminRepAsync(StatusByAdminRepVm statusByAdminRepVm)
    {
        var userId = await _claimsService.GetUserIdFromClaimsAsync();
        var user = await _userRepository.GetUserByIdAsync(userId);
        var constituencyName = await _constituencyRepository.GetConstituencyNameByIdAsync(user!.ConstituencyId);
        var complaints = await _complaintRepository.GetComplaintsByConstituency(constituencyName!);

        // Checking if the complaint exists in the list of complaints for this admin rep's constituency
        if (!complaints.Any(c => c.ComplaintId == statusByAdminRepVm.ComplaintId))
            throw new WelfareWorkTrackerException("You can validate only complaints from your constituency.");

        var currentComplaint = await _complaintRepository.GetComplaintByComplaintIdAsync(statusByAdminRepVm.ComplaintId) 
            ?? throw new WelfareWorkTrackerException("Complaint not found.");

        // If admin rep provides a reference number to link the complaint to an existing one
        if (statusByAdminRepVm.ReferenceNumber != 0)
        {
            // Linking the current complaint by updating its reference number
            currentComplaint.ReferenceNumber = statusByAdminRepVm.ReferenceNumber;
            await _complaintRepository.UpdateComplaintByIdAsync(currentComplaint);

            // Notifying the citizen that their complaint has been linked
            var linkedComplaintStatus = await _complaintStatusRepository.GetComplaintStatusAsync(statusByAdminRepVm.ReferenceNumber) 
                    ?? throw new WelfareWorkTrackerException("No linked complaint's status found");

            if (linkedComplaintStatus.Status == (int)Status.Unresolved)
            {
                var currentComplaintStatus = await _complaintStatusRepository.GetComplaintStatusAsync(statusByAdminRepVm.ComplaintId);
                if (currentComplaintStatus == null)
                    return null;

                // admin rep can only update when the status is in under validating state
                if (currentComplaintStatus.Status != (int)Status.UnderValidation && currentComplaintStatus.Status != (int)Status.Reopened)
                    throw new WelfareWorkTrackerException("You cannot update");

                // Create and save the new status for the current complaint
                var newCurrentStatus = new ComplaintStatus
                {
                    ComplaintId = currentComplaintStatus.ComplaintId,
                    Status = statusByAdminRepVm.Status,
                    AttemptNumber = currentComplaintStatus.AttemptNumber,
                    OpenedDate = currentComplaintStatus.OpenedDate,
                    DeadlineDate = currentComplaintStatus.DeadlineDate,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow
                };

                await _complaintStatusRepository.AddComplaintStatusAsync(newCurrentStatus);

                // notify original user that his complaint got reopened by new user

                var complaint = await _complaintRepository.GetComplaintByComplaintIdAsync(statusByAdminRepVm.ReferenceNumber) 
                    ?? throw new WelfareWorkTrackerException($"Complaint was not Found");

                var citizen = await _userRepository.GetUserByIdAsync(complaint.CitizenId);

                var citizenTemplate = await _emailTemplateRepository.GetByNameAsync(Constants.EmailTemplates.ComplaintCitizenReopen)
                            ?? throw new WelfareWorkTrackerException($"Email Template Not Found!", (int)HttpStatusCode.NotFound);

                var citizenPayload = new Dictionary<string, string> {
                        { "CitizenName", citizen!.FullName},
                        { "ComplaintTitle", complaint.Title}
                    };

                var citizenEmailVm = new EmailVm
                {
                    ToUserEmail = citizen.Email,
                    TemplateId = citizenTemplate.Id,
                    Payload = citizenPayload
                };

                await _emailService.SendEmailAsync(citizenEmailVm);

                return _mapper.Map<ComplaintStatusDto>(newCurrentStatus);
            }

            // notify new user that his complaint is already raised by someone

            var newComplaint = await _complaintRepository.GetComplaintByComplaintIdAsync(statusByAdminRepVm.ComplaintId)
                        ?? throw new WelfareWorkTrackerException($"Complaint was not Found");

            var newCitizen = await _userRepository.GetUserByIdAsync(newComplaint.CitizenId);

            var newComplaintTemplate = await _emailTemplateRepository.GetByNameAsync(Constants.EmailTemplates.ComplaintCitizenExisting)
                        ?? throw new WelfareWorkTrackerException($"Email Template Not Found!", (int)HttpStatusCode.NotFound);

            var newComplaintPayload = new Dictionary<string, string> {
                        { "CitizenName", newCitizen!.FullName},
                        { "ComplaintTitle", newComplaint.Title}
                    };

            var newComplaintVm = new EmailVm
            {
                ToUserEmail = newCitizen.Email,
                TemplateId = newComplaintTemplate.Id,
                Payload = newComplaintPayload
            };
            await _emailService.SendEmailAsync(newComplaintVm);

            var linkedComplaintStatusDto = _mapper.Map<ComplaintStatusDto>(linkedComplaintStatus);
            return linkedComplaintStatusDto;
        }

        // If no reference number is provided, this is a unique complaint and admin rep update's the status

        var complaintStatus = await _complaintStatusRepository.GetComplaintStatusAsync(statusByAdminRepVm.ComplaintId);

        if (complaintStatus == null)
            return null;

        var inUnderValidation = (int)Status.UnderValidation;
        var isReopened = (int)Status.Reopened;

        if (complaintStatus.Status != inUnderValidation && complaintStatus.Status != isReopened)
            throw new WelfareWorkTrackerException("You cannot update");  // admin rep can only update when the status is in under validating state or ReOpened State


        if (statusByAdminRepVm.Status != (int)Status.Valid && 
            statusByAdminRepVm.Status != (int)Status.Invalid && 
            statusByAdminRepVm.Status != (int)Status.Backlog)
            throw new WelfareWorkTrackerException("Invalid status provided");

        ComplaintStatus newStatus;

        if ((complaintStatus.Status == (int)Status.UnderValidation && statusByAdminRepVm.Status == (int)Status.Valid) || 
            (complaintStatus.Status == (int)Status.UnderValidation && statusByAdminRepVm.Status == (int)Status.Invalid) || 
            (complaintStatus.Status == (int)Status.Reopened && statusByAdminRepVm.Status == (int)Status.Backlog))
        {
            newStatus = new ComplaintStatus
            {
                ComplaintId = complaintStatus.ComplaintId,
                AttemptNumber = complaintStatus.AttemptNumber,
                Status = statusByAdminRepVm.Status,
                RejectReason = statusByAdminRepVm.RejectReason,
                OpenedDate = complaintStatus.OpenedDate,
                DateUpdated = DateTime.UtcNow,
                DateCreated = complaintStatus.DateCreated,
            };

            await _complaintStatusRepository.AddComplaintStatusAsync(newStatus);

            var complaint = await _complaintRepository.GetComplaintByComplaintIdAsync(statusByAdminRepVm.ComplaintId) 
                            ?? throw new WelfareWorkTrackerException($"Complaint was not Found");

            var admin = await _userRepository.GetAdminAsync() 
                        ?? throw new WelfareWorkTrackerException($"Admin User was not Found");

            var citizen = await _userRepository.GetUserByIdAsync(complaint.CitizenId);

            var adminRepresentative = await _userRepository.GetAdminRepByConstituencyIdAsync(complaint.ConstituencyId);

            // notify citizen and admin that a complaint got validated by AR
            if (statusByAdminRepVm.Status == (int)Status.Valid)
            {
                var citizenTemplate = await _emailTemplateRepository.GetByNameAsync(Constants.EmailTemplates.ComplaintCitizenValid)
                        ?? throw new WelfareWorkTrackerException($"Email Template Not Found!", (int)HttpStatusCode.NotFound);

                var adminTemplate = await _emailTemplateRepository.GetByNameAsync(Constants.EmailTemplates.ComplaintAdminValid) 
                        ?? throw new WelfareWorkTrackerException("Email template Not Found!");

                var citizenPayload = new Dictionary<string, string> {
                        { "CitizenName", citizen!.FullName},
                        { "ComplaintTitle",complaint.Title}
                    };
                var adminPayload = new Dictionary<string, string> {
                        { "AdminName", admin.FullName},
                        { "AdminRepName", adminRepresentative!.FullName},
                        { "ComplaintTitle", complaint.Title}
                    };

                var citizenEmailVm = new EmailVm
                {
                    ToUserEmail = citizen.Email,
                    TemplateId = citizenTemplate.Id,
                    Payload = citizenPayload
                };

                var adminEmailVm = new EmailVm
                {
                    ToUserEmail = admin.Email,
                    TemplateId = adminTemplate.Id,
                    Payload = adminPayload
                };

                await _emailService.SendEmailAsync(citizenEmailVm);
                await _emailService.SendEmailAsync(adminEmailVm);
            }
            // notify   admin that a complaint was marked as Invalid by AR
            else if (statusByAdminRepVm.Status == (int)Status.Invalid)
            {

                var adminTemplate = await _emailTemplateRepository.GetByNameAsync(Constants.EmailTemplates.ComplaintAdminInvalid) ??
                    throw new WelfareWorkTrackerException("Email template Not Found!");

                var adminPayload = new Dictionary<string, string> {
                        { "AdminName", admin.FullName},
                        { "AdminRepName", adminRepresentative!.FullName},
                        { "ComplaintTitle", complaint.Title}
                };

                var adminEmailVm = new EmailVm
                {
                    ToUserEmail = admin.Email,
                    TemplateId = adminTemplate.Id,
                    Payload = adminPayload
                };

                await _emailService.SendEmailAsync(adminEmailVm);
            }

        }
        else
        {
            throw new WelfareWorkTrackerException(complaintStatus.Status == (int)Status.UnderValidation
            ? $"Status should be Valid or Invalid, you cannot update to {Enum.GetName(typeof(Status), statusByAdminRepVm.Status)}"
            : $"Status should be Backlog, you cannot update to {Enum.GetName(typeof(Status), statusByAdminRepVm.Status)}");
        }
        
        var complaintStatusDto = _mapper.Map<ComplaintStatusDto>(newStatus);
        return complaintStatusDto;
    }

    public async Task<ComplaintStatusDto?> GetComplaintStatusByComplaintId(int complaintId)
    {
        var complaintStatus = await _complaintStatusRepository.GetComplaintStatusAsync(complaintId);
        if (complaintStatus == null)
            return null;

        return _mapper.Map<ComplaintStatusDto>(complaintStatus);
    }

    public async Task<ComplaintStatusDto?> GetComplaintStatusByIdAsync(int complaintStatusId)
    {
        var complaintStatus = await _complaintStatusRepository.GetComplaintStatusAsync(null, complaintStatusId);
        if (complaintStatus == null)
            return null;

        return _mapper.Map<ComplaintStatusDto>(complaintStatus);
    }

    public async Task<List<ComplaintStatusDto>> GetComplaintStatusHistoryAsync(int complaintId)
    {
        var complaintStatusHistory = await _complaintStatusRepository.GetComplaintStatusHistoryByComplaintIdAsync(complaintId);

        return _mapper.Map<List<ComplaintStatusDto>>(complaintStatusHistory);
    }

    public async Task<ComplaintStatusDto?> UpdateComplaintStatusByLeaderAsync(StatusByLeaderVm statusUpdateVm)
    {
        var userId = await _claimsService.GetUserIdFromClaimsAsync();
        var user = await _userRepository.GetUserByIdAsync(userId);
        var leader = await _userRepository.GetLeaderByConstituencyIdAsync(user!.ConstituencyId);

        if (leader!.Reputation <= 15)
        {
            // Stop further processing for this leader
            throw new WelfareWorkTrackerException("You cannot approve as your reputation dropped below 15%");
        }

        var constituencyName = await _constituencyRepository.GetConstituencyNameByIdAsync(user.ConstituencyId);
        var complaints = await _complaintRepository.GetComplaintsByConstituency(constituencyName!);

        // Checking if the complaint exists in the list of complaints for this leader constituency
        if (!complaints.Any(c => c.ComplaintId == statusUpdateVm.ComplaintId))
            throw new WelfareWorkTrackerException("You can validate only complaints from your constituency.");

        // Fetch the current complaint status from the repository
        var complaintStatus = await _complaintStatusRepository.GetComplaintStatusAsync(statusUpdateVm.ComplaintId);

        // If no complaint status is found, return null
        if (complaintStatus == null)
            return null;

        if (statusUpdateVm.Status == (int)Status.Approve) // Approve
        {
            // Checking the current status is 'Backlog' (4) before making updates
            if (complaintStatus.Status != (int)Status.Backlog)
                throw new WelfareWorkTrackerException("The complaint must be in 'Backlog' (4) status to be approved.");

            // DateTime currentTime = new DateTime(2024, 10, 6, 11, 32, 0)
            DateTime currentTime = DateTime.Now;
            TimeSpan totalDuration = currentTime - complaintStatus.OpenedDate!.Value;

            if (totalDuration.TotalDays > 6)
                throw new WelfareWorkTrackerException("You cannot approve the complaint as more than 6 days have passed.");

            var newStatusRecord = new ComplaintStatus
            {
                ComplaintId = complaintStatus.ComplaintId,
                AttemptNumber = complaintStatus.AttemptNumber,
                Status = 7, // Approved, (7) - in progress stage
                DateUpdated = DateTime.UtcNow,
                DateCreated = complaintStatus.DateCreated,
                DeadlineDate = statusUpdateVm.DeadlineDate,
                OpenedDate = complaintStatus.OpenedDate
            };

            await _complaintStatusRepository.AddComplaintStatusAsync(newStatusRecord);

            // notify citizen when it is approved by leader
            var complaint = await _complaintRepository.GetComplaintByComplaintIdAsync(statusUpdateVm.ComplaintId) 
                ?? throw new WelfareWorkTrackerException("Complaint was Not Found!");
            var citizen = await _userRepository.GetUserByIdAsync(complaint.CitizenId);

            var citizenApprovalTemplate = await _emailTemplateRepository.GetByNameAsync(Constants.EmailTemplates.ComplaintCitizenApproval) 
                    ?? throw new WelfareWorkTrackerException("Email Template Not Found!");


            var citizenApprovalPayload = new Dictionary<string, string> {
                  { "CitizenName", citizen!.FullName},
                  { "LeaderName", leader.FullName},
                  { "ComplaintTitle",complaint.Title}
            };

            var citizenEmailVm = new EmailVm
            {
                ToUserEmail = citizen.Email,
                TemplateId = citizenApprovalTemplate.Id,
                Payload = citizenApprovalPayload
            };

            await _emailService.SendEmailAsync(citizenEmailVm);

            return _mapper.Map<ComplaintStatusDto>(newStatusRecord);
        }
        else if (statusUpdateVm.Status == (int)Status.Resolved) // Resolve
        {
            // Checking if the current status is InProgress (7)
            if (complaintStatus.Status != (int)Status.InProgress)
                throw new WelfareWorkTrackerException("Complaint must be in 'InProgress' (7) status to resolve.");

            var complaint = await _complaintRepository.GetComplaintByComplaintIdAsync(complaintStatus.ComplaintId) 
                    ?? throw new WelfareWorkTrackerException($"No complaint found with complaintId: {complaintStatus.ComplaintId}");
            var attemptNumber = complaint.Attempts;

            var resolvedStatusRecord = new ComplaintStatus
            {
                ComplaintId = complaintStatus.ComplaintId,
                AttemptNumber = attemptNumber,
                Status = 8, // Resolved
                OpenedDate = complaintStatus.DateCreated,
                DeadlineDate = complaintStatus.DeadlineDate,
                DateUpdated = DateTime.UtcNow,
                DateCreated = complaintStatus.DateCreated,
            };

            await _complaintStatusRepository.AddComplaintStatusAsync(resolvedStatusRecord);

            // notify citizen that complaint got resolved
            var citizen = await _userRepository.GetUserByIdAsync(complaint.CitizenId);

            var citizenResolvedTemplate = await _emailTemplateRepository.GetByNameAsync(Constants.EmailTemplates.ComplaintCitizenResolved)
                        ?? throw new WelfareWorkTrackerException("Email Template Not Found!");

            var citizenResolvedPayload = new Dictionary<string, string> {
                  { "CitizenName", citizen!.FullName},
                  { "LeaderName", leader.FullName},
                  { "ComplaintTitle",complaint.Title}
            };

            var citizenResolvedEmailVm = new EmailVm
            {
                ToUserEmail = citizen.Email,
                TemplateId = citizenResolvedTemplate.Id,
                Payload = citizenResolvedPayload
            };

            await _emailService.SendEmailAsync(citizenResolvedEmailVm);

            DateTime scheduledTime = DateTime.Now.AddDays(10);

            // Scheduling the job, injecting the service via Hang fire
            BackgroundJob.Schedule<IFeedbackService>(
                service => service.EvaluateFeedback(statusUpdateVm.ComplaintId),
                scheduledTime
            );

            // notify all citizens in that constituency to give feedback

            var citizens = await _userRepository.GetUsersByConstituencyIdAsync(complaint.ConstituencyId);

            var constituencyResolvedTemplate = await _emailTemplateRepository.GetByNameAsync(Constants.EmailTemplates.ComplaintConstituencyResolved)
                        ?? throw new WelfareWorkTrackerException("Email Template Not Found!");

            foreach (var c in citizens!)
            {
                if (c.UserId == leader.UserId || c.UserId == complaint.CitizenId)
                {
                    continue;
                }
                var payload = new Dictionary<string, string>
                {
                    { "CitizenName", citizen!.FullName},
                    { "LeaderName", leader.FullName},
                    { "ComplaintTitle",complaint.Title},
                    { "ConstituencyName", complaint.ConstituencyName}
                };

                var emailVm = new EmailVm
                {
                    ToUserEmail = c.Email,
                    TemplateId = constituencyResolvedTemplate.Id,
                    Payload = payload
                };

                await _emailService.SendEmailAsync(emailVm);
            }

            return _mapper.Map<ComplaintStatusDto>(resolvedStatusRecord);
        }
        else
        {
            throw new ArgumentException("Invalid status provided.");
        }
    }
}
