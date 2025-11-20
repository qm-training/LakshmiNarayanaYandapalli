namespace WelfareWorkTracker.Infrastructure.Service;
public class FeedbackService(IFeedbackRepository FeedbackRepository,
                                IComplaintRepository complaintRepository,
                                IDailyComplaintRepository dailyComplaintRepository,
                                IComplaintStatusRepository complaintStatusRepository,
                                IDailyComplaintStatusRepository dailyComplaintStatusRepository,
                                IClaimsService claimsService, IUserRepository userRepository,
                                IEmailTemplateRepository emailTemplateRepository,
                                IEmailService emailService,
                                IMapper mapper) : IFeedbackService
{
    private readonly IFeedbackRepository _FeedbackRepository = FeedbackRepository;
    private readonly IComplaintRepository _complaintRepository = complaintRepository;
    private readonly IDailyComplaintRepository _dailyComplaintRepository = dailyComplaintRepository;
    private readonly IComplaintStatusRepository _complaintStatusRepository = complaintStatusRepository;
    private readonly IDailyComplaintStatusRepository _dailyComplaintStatusRepository = dailyComplaintStatusRepository;
    private readonly IClaimsService _claimsService = claimsService;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IEmailTemplateRepository _emailTemplateRepository = emailTemplateRepository;
    private readonly IEmailService _emailService = emailService;
    private readonly IMapper _mapper = mapper;

    public async Task<FeedbackDto> AddFeedbackAsync(FeedbackVm citizenFeedbackVm)
    {
        if (!citizenFeedbackVm.ComplaintId.HasValue && !citizenFeedbackVm.DailyComplaintId.HasValue)
            throw new WelfareWorkTrackerException("Any one of the ID's should be provided.", (int)HttpStatusCode.BadRequest);

        else if (citizenFeedbackVm.ComplaintId.HasValue && citizenFeedbackVm.DailyComplaintId.HasValue)
            throw new WelfareWorkTrackerException("only one of the ID's should be provided.", (int)HttpStatusCode.BadRequest);

        // Validate the user ID from claims
        var userId = await _claimsService.GetUserIdFromClaimsAsync();

        // Check if the complaint exists based on ComplaintId or DailyComplaintId
        var complaint = citizenFeedbackVm.ComplaintId.HasValue
            ? await _complaintRepository.GetComplaintByComplaintIdAsync(citizenFeedbackVm.ComplaintId.Value) : null;

        var dailyComplaint = citizenFeedbackVm.DailyComplaintId.HasValue
                ? await _dailyComplaintRepository.GetDailyComplaintByIdAsync(citizenFeedbackVm.DailyComplaintId.Value)
                : null;

        if (citizenFeedbackVm.ComplaintId > 0 && complaint == null)
            throw new WelfareWorkTrackerException("complaint not found", (int)HttpStatusCode.NotFound);

        if (citizenFeedbackVm.DailyComplaintId > 0 && dailyComplaint == null)
            throw new WelfareWorkTrackerException("daily complaint not found", (int)HttpStatusCode.NotFound);

        // Validate that the user's constituency matches the complaint's constituency
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (complaint != null && citizenFeedbackVm.ComplaintId.HasValue && complaint.ConstituencyId != user!.ConstituencyId)
            throw new WelfareWorkTrackerException("You can only provide feedback for your constituency.", (int)HttpStatusCode.Unauthorized);

        // Check if the user has already provided feedback for the complaint
        var existingFeedback = await _FeedbackRepository.GetFeedbackByUserAsync(userId, citizenFeedbackVm.ComplaintId, citizenFeedbackVm.DailyComplaintId);

        if (existingFeedback != null)
            throw new WelfareWorkTrackerException("You have already submitted feedback for this complaint.", (int)HttpStatusCode.BadRequest);

        // Ensure the complaint's status is Resolved before allowing feedback
        if (complaint != null && citizenFeedbackVm.ComplaintId > 0)
        {
            var complaintStatus = await _complaintStatusRepository.GetComplaintStatusAsync(complaint.ComplaintId);
            if (complaintStatus == null || complaintStatus.Status != (int)Status.Resolved)
                throw new WelfareWorkTrackerException("You can provide feedback only after the complaint is resolved.", (int)HttpStatusCode.BadRequest);
        }

        if (dailyComplaint != null && citizenFeedbackVm.DailyComplaintId > 0)
        {
            var complaintStatus = await _dailyComplaintStatusRepository.GetDailyComplaintStatusAsync(dailyComplaint.DailyComplaintId);

            if (complaintStatus == null || complaintStatus.Status != (int)Status.Resolved)
                throw new WelfareWorkTrackerException("You can provide feedback only after the complaint is resolved.", (int)HttpStatusCode.BadRequest);
        }
        // Create the complaint feedback
        var complaintFeedback = new ComplaintFeedback
        {
            CitizenId = userId,
            ComplaintId = citizenFeedbackVm.ComplaintId,
            DailyComplaintId = citizenFeedbackVm.DailyComplaintId,
            FeedbackMessage = citizenFeedbackVm.FeedbackMessage,
            IsSatisfied = citizenFeedbackVm.IsSatisfied,
            DateCreated = DateTime.UtcNow,
        };

        // Save the feedback
        var addedComplaintFeedback = await _FeedbackRepository.AddFeedbackAsync(complaintFeedback);

        return _mapper.Map<FeedbackDto>(addedComplaintFeedback);
    }
    public async Task<List<FeedbackDto>?> GetAllFeedbacksAsync(int? complaintId, int? dailyComplaintId)
    {
        if (!complaintId.HasValue && !dailyComplaintId.HasValue)
            throw new WelfareWorkTrackerException("Any one of the ID's should be provided.", (int)HttpStatusCode.BadRequest);

        else if (complaintId.HasValue && dailyComplaintId.HasValue)
            throw new WelfareWorkTrackerException("only one of the ID's should be provided.", (int)HttpStatusCode.BadRequest);

        var userId = await _claimsService.GetUserIdFromClaimsAsync();
        var complaintFeedbacks = await _FeedbackRepository.GetAllFeedbacksAsync(complaintId, dailyComplaintId);
        if (complaintFeedbacks == null)
            return null;

        var orderedFeedbacks = complaintFeedbacks.OrderByDescending(f => f.CitizenId == userId).ToList();

        var feedbackDtos = new List<FeedbackDto>();

        foreach (var feedback in orderedFeedbacks)
        {
            var dto = _mapper.Map<FeedbackDto>(feedback);
            feedbackDtos.Add(dto);
        }
        return feedbackDtos;
    }

    public async Task<FeedbackDto?> GetFeedbackByUserAsync(int? complaintId, int? dailyComplaintId)
    {
        if (!complaintId.HasValue && !dailyComplaintId.HasValue)
            throw new WelfareWorkTrackerException("Any one of the ID's should be provided.", (int)HttpStatusCode.BadRequest);

        else if (complaintId.HasValue && dailyComplaintId.HasValue)
            throw new WelfareWorkTrackerException("only one of the ID's should be provided.", (int)HttpStatusCode.BadRequest);

        var userId = await _claimsService.GetUserIdFromClaimsAsync();
        var complaintFeedback = await _FeedbackRepository.GetFeedbackByUserAsync(userId, complaintId, dailyComplaintId);
        if (complaintFeedback == null)
            return new FeedbackDto();

        return _mapper.Map<FeedbackDto>(complaintFeedback);
    }

    public async Task EvaluateFeedback(int complaintId)
    {
        var complaint = await _complaintRepository.GetComplaintByComplaintIdAsync(complaintId)
            ?? throw new WelfareWorkTrackerException($"No complaint found with complaintId: {complaintId}");

        var complaintStatus = await _complaintStatusRepository.GetComplaintStatusAsync(complaintId);

        var leader = await _userRepository.GetLeaderByConstituencyIdAsync(complaint.ConstituencyId);

        if (leader!.Reputation <= 15)
        {
            // No further process for this leader
            return;
        }

        // Checking if the complaint is resolved
        if (complaintStatus!.Status == (int)Status.Resolved)
        {
            int population = await _userRepository.GetCitizenCountInConstituencyAsync(complaint.ConstituencyId);

            float satisfiedThreshold = population * 0.25f;
            float unsatisfiedThreshold = satisfiedThreshold * 0.10f;
            float satisfiedCount = await _FeedbackRepository.GetSatisfiedCount(complaintId);
            float unsatisfiedCount = await _FeedbackRepository.GetUnSatisfiedCount(complaintId);

            // Checking if 10 days have passed since the complaint was resolved
            if (DateTime.UtcNow >= complaintStatus.DateUpdated)
            {
                float satisfiedFeedback = (satisfiedCount / satisfiedThreshold) * (satisfiedCount * 0.01f * population);
                float unsatisfiedFeedback = (unsatisfiedCount / unsatisfiedThreshold) * (unsatisfiedCount * 0.01f * population);

                // Determining the final feedback status
                if (satisfiedFeedback > unsatisfiedFeedback)
                {
                    await AddComplaintStatusRecordAsync(complaintId, Status.Closed);
                    int value = (int)Math.Ceiling(satisfiedCount * 0.01f * population);

                    var previousReputation = leader.Reputation;

                    if (leader.Reputation < 100)
                    {
                        leader.Reputation += value * 0.01;

                        if (leader.Reputation > 100)
                            leader.Reputation = 100;

                        await _userRepository.UpdateLeaderReputationAsync(leader.UserId, leader.Reputation);
                    }
                    // notify citizen and leader about complaint closed

                    var citizen = await _userRepository.GetUserByIdAsync(complaint.CitizenId);

                    var leaderClosedTemplate = await _emailTemplateRepository.GetByNameAsync(Constants.EmailTemplates.ComplaintLeaderClosed);
                    var citizenClosedTemplate = await _emailTemplateRepository.GetByNameAsync(Constants.EmailTemplates.ComplaintCitizenClosed);


                    var leaderClosedPayload = new Dictionary<string, string>
                        {
                          {"LeaderName",leader.FullName},
                          {"ComplaintTitle",complaint.Title }

                        };

                    var citizenClosedPayload = new Dictionary<string, string>
                        {
                          {"CitizenName",citizen!.FullName},
                          {"LeaderName",leader.FullName},
                          {"ComplaintTitle",complaint.Title }

                        };

                    var leaderClosedEmailVm = new EmailVm
                    {
                        ToUserEmail = leader.Email,
                        TemplateId = leaderClosedTemplate.Id,
                        Payload = leaderClosedPayload
                    };

                    var citizenClosedEmailVm = new EmailVm
                    {
                        ToUserEmail = citizen.Email,
                        TemplateId = citizenClosedTemplate.Id,
                        Payload = citizenClosedPayload
                    };

                    await _emailService.SendEmailAsync(leaderClosedEmailVm);
                    await _emailService.SendEmailAsync(citizenClosedEmailVm);

                    // notify leader that his reputation got increased
                    var leaderReputationIncreasedTemplate = await _emailTemplateRepository.GetByNameAsync(Constants.EmailTemplates.LeaderReputationIncrease);

                    var leaderReputationIncreasedPayload = new Dictionary<string, string>
                            {
                              {"LeaderName", leader.FullName},
                              {"PreviousReputation",previousReputation.ToString() },
                              {"NewReputation",(leader.Reputation) .ToString() }
                            };

                    var leaderReputationIncreasedEmailVm = new EmailVm
                    {
                        ToUserEmail = leader.Email,
                        TemplateId = leaderReputationIncreasedTemplate.Id,
                        Payload = leaderReputationIncreasedPayload
                    };


                    await _emailService.SendEmailAsync(leaderReputationIncreasedEmailVm);

                }
                else if (unsatisfiedFeedback > satisfiedFeedback)
                {
                    // Incrementing attempts if unsatisfied feedback is higher
                    complaint.Attempts++;

                    if (complaint.Attempts >= 4)
                    {
                        // Marking complaint as 'Unresolved' if attempts exceed 3
                        await AddComplaintStatusRecordAsync(complaintId, Status.Unresolved);

                        var unResolvedComplaints = await _complaintStatusRepository.GetUnresolvedComplaintsOfLeader(leader.UserId, Status.Unresolved);

                        int unResolvedCount = 0;

                        foreach (var UnResolvedComplaint in unResolvedComplaints)
                        {
                            unResolvedCount++;
                        }

                        if (unResolvedCount > 3)
                        {
                            var previousReputation = leader.Reputation;
                            leader.Reputation -= 50;
                            if (leader.Reputation < 0)
                            {
                                leader.Reputation = 0;
                            }
                            // notify leader that his reputation decreased to 50%

                            var leaderReputationReducedTemplate = await _emailTemplateRepository.GetByNameAsync(Constants.EmailTemplates.LeaderReputationDecrease);

                            var leaderReputationReducedPayload = new Dictionary<string, string>
                            {
                              {"LeaderName", leader.FullName},
                              {"PreviousReputation",previousReputation.ToString() },
                              {"NewReputation",(leader.Reputation) .ToString() }
                            };

                            var leaderReputationReducedEmailVm = new EmailVm
                            {
                                ToUserEmail = leader.Email,
                                TemplateId = leaderReputationReducedTemplate.Id,
                                Payload = leaderReputationReducedPayload
                            };

                            await _emailService.SendEmailAsync(leaderReputationReducedEmailVm);

                        }
                        else
                        {
                            var previousReputation = leader.Reputation;

                            leader.Reputation -= 40;
                            // notify leader about reputation decreased to 40%(UnResolved)
                            var leaderReputationReducedTemplate = await _emailTemplateRepository.GetByNameAsync(Constants.EmailTemplates.LeaderReputationDecrease);

                            var leaderReputationReducedPayload = new Dictionary<string, string>
                            {
                              {"LeaderName", leader.FullName},
                              {"PreviousReputation",previousReputation.ToString() },
                              {"NewReputation",(leader.Reputation) .ToString() }
                            };

                            var leaderReputationReducedEmailVm = new EmailVm
                            {
                                ToUserEmail = leader.Email,
                                TemplateId = leaderReputationReducedTemplate.Id,
                                Payload = leaderReputationReducedPayload
                            };

                            await _emailService.SendEmailAsync(leaderReputationReducedEmailVm);
                        }


                        // notify citizen and leader about complaint unResolved
                        var citizen = await _userRepository.GetUserByIdAsync(complaint.CitizenId);

                        var leaderUnResolvedTemplate = await _emailTemplateRepository.GetByNameAsync(Constants.EmailTemplates.ComplaintLeaderUnresolved);
                        var citizenUnResolvedTemplate = await _emailTemplateRepository.GetByNameAsync(Constants.EmailTemplates.ComplaintCitizenUnresolved);

                        var leaderUnResolvedPayload = new Dictionary<string, string>
                        {
                              {"LeaderName", leader.FullName},
                              {"complaintTitle",complaint.Title }

                        };

                        var citizenUnResolvedPayload = new Dictionary<string, string>
                        {
                          {"CitizenName",citizen!.FullName},
                          {"ComplaintTitle",complaint.Title }

                        };

                        var leaderUnResolvedEmailVm = new EmailVm
                        {
                            ToUserEmail = leader.Email,
                            TemplateId = leaderUnResolvedTemplate.Id,
                            Payload = leaderUnResolvedPayload
                        };

                        var citizenUnResolvedEmailVm = new EmailVm
                        {
                            ToUserEmail = citizen.Email,
                            TemplateId = citizenUnResolvedTemplate.Id,
                            Payload = citizenUnResolvedPayload
                        };

                        await _emailService.SendEmailAsync(leaderUnResolvedEmailVm);
                        await _emailService.SendEmailAsync(citizenUnResolvedEmailVm);
                    }
                    else
                    {
                        // Reopening the complaint
                        complaint.Attempts++;

                        await AddComplaintStatusRecordAsync(complaintStatus.ComplaintId, Status.Reopened);

                        // notify citizen and leader that complaint got reopened
                        var citizen = await _userRepository.GetUserByIdAsync(complaint.CitizenId);

                        var leaderReOpenedTemplate = await _emailTemplateRepository.GetByNameAsync(Constants.EmailTemplates.ComplaintLeaderReopened);
                        var citizenReOpenedTemplate = await _emailTemplateRepository.GetByNameAsync(Constants.EmailTemplates.ComplaintLeaderReopened);

                        var leaderReOpenedPayload = new Dictionary<string, string>
                        {
                          {"LeaderName", leader.FullName},
                          {"ComplaintTitle", complaint.Title }
                        };

                        var citizenReOpenedPayload = new Dictionary<string, string>
                        {
                          {"CitizenName", citizen!.FullName},
                          {"complaintTitle", complaint.Title }
                        };

                        var leaderReOpenedEmailVm = new EmailVm
                        {
                            ToUserEmail = leader.Email,
                            TemplateId = leaderReOpenedTemplate.Id,
                            Payload = leaderReOpenedPayload
                        };

                        var citizenReOpenedEmailVm = new EmailVm
                        {
                            ToUserEmail = citizen.Email,
                            TemplateId = citizenReOpenedTemplate.Id,
                            Payload = citizenReOpenedPayload
                        };

                        await _emailService.SendEmailAsync(leaderReOpenedEmailVm);
                        await _emailService.SendEmailAsync(citizenReOpenedEmailVm);
                    }

                    await _complaintRepository.UpdateComplaintByIdAsync(complaint);

                    var attemptNumber = complaint.Attempts;

                    // Decrease reputation based on the attempt number
                    if (attemptNumber == 2 || attemptNumber == 3 || attemptNumber == 4)
                    {
                        await HandleReputationReductionAsync(attemptNumber, leader, unsatisfiedCount, population);
                    }

                    // Updating the leader's information
                    await _userRepository.UpdateLeaderReputationAsync(leader.UserId, leader.Reputation);

                    if (leader.Reputation < 15)
                    {
                        //await TriggerReElectionAsync(leader)
                    }
                }
            }
            else
            {
                throw new WelfareWorkTrackerException();
            }
        }
    }

    private async Task HandleReputationReductionAsync(int attemptNumber, User leader, float unsatisfiedCount, int population)
    {   
        int value = (int)Math.Ceiling(unsatisfiedCount * 0.01f * population);
        double reductionMultiplier = Math.Pow(2, attemptNumber - 2);

        var previousReputation = leader.Reputation;
        leader.Reputation -= reductionMultiplier * value * 0.01;
        var template = await _emailTemplateRepository.GetByNameAsync(Constants.EmailTemplates.LeaderReputationDecrease);

        var payload = new Dictionary<string, string>
        {
            {"LeaderName", leader.FullName},
            {"PreviousReputation",previousReputation.ToString() },
            {"NewReputation",(leader.Reputation) .ToString() }
        };

        var emailVm = new EmailVm
        {
            ToUserEmail = leader.Email,
            TemplateId = template.Id,
            Payload = payload
        };

        await _emailService.SendEmailAsync(emailVm);
    }

    public async Task EvaluateDailyComplaintFeedback()
    {
        var todaysDailyComplaint = await _dailyComplaintRepository.GetTodaysDailyComplaintsAsync();
        foreach (var dailyComplaint in todaysDailyComplaint)
        {
            var dailyComplaintStatus = await _dailyComplaintStatusRepository.GetDailyComplaintStatusAsync(dailyComplaint.DailyComplaintId)
                                        ?? throw new WelfareWorkTrackerException();

            var leader = await _userRepository.GetLeaderByConstituencyIdAsync(dailyComplaint.ConstituencyId);

            if (leader!.Reputation <= 15)
            {
                continue;
            }

            // Checking if the complaint is resolved
            if (dailyComplaintStatus.Status == (int)Status.Resolved)
            {
                int population = await _userRepository.GetCitizenCountInConstituencyAsync(dailyComplaint.ConstituencyId);

                float satisfiedThreshold = population * 0.25f;
                float unsatisfiedThreshold = satisfiedThreshold * 0.10f;

                float satisfiedCount = await _FeedbackRepository.GetSatisfiedCount(null, dailyComplaint.DailyComplaintId);
                float unsatisfiedCount = await _FeedbackRepository.GetUnSatisfiedCount(null, dailyComplaint.DailyComplaintId);

                float satisfiedFeedback = (satisfiedCount / satisfiedThreshold) * (satisfiedCount * 0.01f * population);
                float unsatisfiedFeedback = (unsatisfiedCount / unsatisfiedThreshold) * (unsatisfiedCount * 0.01f * population);

                if (satisfiedFeedback > unsatisfiedFeedback)
                {
                    int value = (int)Math.Ceiling(satisfiedCount * 0.01f * population);

                    leader.Reputation += value * 0.01;

                    if (leader.Reputation > 100)
                        leader.Reputation = 100;
                }
                else if (unsatisfiedFeedback > satisfiedFeedback)
                {
                    var unResolvedComplaints = await _dailyComplaintStatusRepository.GetUnresolvedComplaintsOfLeader(leader.UserId, Status.Unresolved);

                    int unResolvedCount = 0;

                    foreach (var UnResolvedComplaint in unResolvedComplaints)
                    {
                        unResolvedCount++;
                    }

                    if (unResolvedCount > 3)
                    {
                        leader.Reputation -= 5;
                    }
                    else
                    {
                        int value = (int)Math.Ceiling(unsatisfiedCount * 0.01f * population);

                        leader.Reputation -= Math.Min(value * 0.01, 5);
                    }
                }

                // Updating the leader's information
                await _userRepository.UpdateLeaderReputationAsync(leader.UserId, leader.Reputation);

                if (leader.Reputation < 15)
                {
                    //await TriggerReElectionAsync(leader)
                }
            }
        }
    }

    private async Task AddComplaintStatusRecordAsync(int complaintId, Status newStatus)
    {
        var complaint = await _complaintRepository.GetComplaintByComplaintIdAsync(complaintId) 
            ?? throw new WelfareWorkTrackerException($"No complaint found with complaint Id: {complaintId}");
        var newStatusRecord = new ComplaintStatus
        {
            ComplaintId = complaintId,
            Status = (int)newStatus,
            OpenedDate = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow,
            AttemptNumber = complaint.Attempts,
        };

        // Add the new record to the ComplaintStatus table
        await _complaintStatusRepository.AddComplaintStatusAsync(newStatusRecord);
    }
}