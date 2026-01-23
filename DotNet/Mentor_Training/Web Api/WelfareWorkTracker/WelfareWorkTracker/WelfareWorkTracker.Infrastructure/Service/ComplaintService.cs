namespace WelfareWorkTracker.Infrastructure.Service;
public class ComplaintService(IComplaintRepository complaintRepository,
                                IUserRepository userRepository,
                                IComplaintImageRepository complaintImageRepository,
                                IComplaintStatusRepository complaintStatusRepository,
                                IConstituencyRepository constituencyRepository,
                                IClaimsService claimsService,
                                IMapper mapper,
                                IEmailService emailService,
                                IEmailTemplateRepository emailTemplateRepository) : IComplaintService
{
    private readonly IComplaintRepository _complaintRepository = complaintRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IComplaintImageRepository _complaintImageRepository = complaintImageRepository;
    private readonly IComplaintStatusRepository _complaintStatusRepository = complaintStatusRepository;
    private readonly IConstituencyRepository _constituencyRepository = constituencyRepository;
    private readonly IClaimsService _claimsService = claimsService;
    private readonly IMapper _mapper = mapper;
    private readonly IEmailService _emailService = emailService;
    private readonly IEmailTemplateRepository _emailTemplateRepository = emailTemplateRepository;

    public async Task<ComplaintDto> AddComplaintAsync(ComplaintVm complaintVm)
    {
        var constituencyId = await _constituencyRepository.GetConstituencyIdByNameAsync(complaintVm.ConstituencyName);
        var leader = await _userRepository.GetLeaderByConstituencyIdAsync(constituencyId);

        var userId = await _claimsService.GetUserIdFromClaimsAsync();

        if (complaintVm.Images == null || complaintVm.Images.Count == 0)
            throw new WelfareWorkTrackerException("Atleast one image is required");

        var complaint = new Complaint
        {
            Title = complaintVm.Title,
            Description = complaintVm.Description,
            ConstituencyName = complaintVm.ConstituencyName,
            DateCreated = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow,
            LeaderId = leader!.UserId,
            CitizenId = userId,
            ConstituencyId = constituencyId
        };

        var addedComplaint = await _complaintRepository.AddComplaintAsync(complaint);

        var imageUrls = new List<string>();

        // To handle images

        foreach (var imageUrl in complaintVm.Images)
        {
            var complaintImage = new ComplaintImage
            {
                ImageUrl = imageUrl,
                ComplaintId = complaint.ComplaintId,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
            };

            var addedImage = await _complaintImageRepository.AddComplaintImageAsync(complaintImage);
            imageUrls.Add(addedImage.ImageUrl);
        }

        var complaintStatus = new ComplaintStatus
        {
            ComplaintId = complaint.ComplaintId,
            AttemptNumber = complaint.Attempts,   // is automatically assigned to 1 when citizen raises a complaint
            DateCreated = DateTime.UtcNow,          // To record the date when the complaint is raised
            Status = (int)Status.UnderValidation, // The status of the complaint will be in underValidating stage
        };

        var addedComplaintStatus = await _complaintStatusRepository.AddComplaintStatusAsync(complaintStatus);

        // Send Complaint Registration Successful email to Citizen
        var citizen = await _userRepository.GetUserByIdAsync(userId);
        var emailTemplate = await _emailTemplateRepository.GetByNameAsync(Constants.EmailTemplates.NewComplaintCitizen) 
                                ?? throw new WelfareWorkTrackerException($"Email Template Not Found!", (int)HttpStatusCode.NotFound);
        var payload = new Dictionary<string, string> {
            { "FullName", citizen!.FullName},
            { "ComplaintTitle", complaint.Title}
            };
        var email = new EmailVm
        {
            ToUserEmail = citizen.Email,
            TemplateId = emailTemplate.Id,
            Payload = payload
        };
        await _emailService.SendEmailAsync(email);

        // Notify Admin Rep of new complaint Email
        var adminRep = await _userRepository.GetAdminRepByConstituencyIdAsync(complaint.ConstituencyId);
        var adminRepEmailTemplate = await _emailTemplateRepository.GetByNameAsync(Constants.EmailTemplates.NewComplaintAdminRep) 
                                        ?? throw new WelfareWorkTrackerException($"Email Template Not Found!", (int)HttpStatusCode.NotFound);
        var adminRepPayload = new Dictionary<string, string>
        {
            { "AdminName", adminRep!.FullName},
            { "CitizenName", citizen!.FullName },
            { "ComplaintTitle", complaint.Title}
        };
        var adminRepEmail = new EmailVm
        {
            ToUserEmail = adminRep.Email,
            TemplateId = adminRepEmailTemplate.Id,
            Payload = adminRepPayload
        };
        await _emailService.SendEmailAsync(adminRepEmail);

        var complaintDto = _mapper.Map<ComplaintDto>(addedComplaint);
        complaintDto.Images = imageUrls;
        complaintDto.Status = ((Status)addedComplaintStatus.Status).ToString();

        return complaintDto;
    }

    public async Task CheckComplaintsForLeaderApprovalAsync()
    {
        var complaintsInBacklog = await _complaintRepository.GetComplaintsInBacklogForMoreThanHoursAsync(72);

        DateTime currentTime = DateTime.UtcNow;

        foreach (var complaint in complaintsInBacklog)
        {
            var complaintRecord = await _complaintRepository.GetComplaintByComplaintIdAsync(complaint!.ComplaintId);
            var leader = await _userRepository.GetLeaderByConstituencyIdAsync(complaintRecord!.ConstituencyId);
            var newReputation = 0.0;

            // Check if leader's reputation is already 15% or lower
            if (leader!.Reputation <= 15)
            {
                continue;
            }

            // Checking if the complaint is still in backlog and not resolved
            if (complaint.Status == (int)Status.Backlog)
            {
                if (complaint.NewOpenedDate == null)
                {
                    // Checking if 3 days have passed since OpenedDate
                    if (currentTime >= complaint.OpenedDate!.Value.AddDays(3))
                    {
                        newReputation = await DecreaseLeaderReputationForApprovalDelayAsync(leader);

                        complaint.NewOpenedDate = currentTime;
                    }
                    await _complaintStatusRepository.UpdateComplaintStatus(complaint);

                    // Notify Leader to approve the backlog complaint , with new open date and his reputation
                    var leaderApprovalTemplate = await _emailTemplateRepository.GetByNameAsync(Constants.EmailTemplates.ComplaintLeaderApprovalDelay);

                    var leaderApprovalPayload = new Dictionary<string, string>
                    {
                      {"LeaderName",leader.FullName},
                      {"ComplaintTitle",complaintRecord.Title },
                      { "PreviousReputation", leader.Reputation.ToString()},
                      { "NewReputation", newReputation.ToString()}

                    };

                    var leaderApprovalDelayEmailVm = new EmailVm
                    {
                        ToUserEmail = leader.Email,
                        TemplateId = leaderApprovalTemplate.Id,
                        Payload = leaderApprovalPayload
                    };

                    await _emailService.SendEmailAsync(leaderApprovalDelayEmailVm);

                }
                else
                {
                    // Calculating total days since the initial OpenedDate
                    TimeSpan totalDaysElapsed = currentTime - complaint.OpenedDate!.Value;

                    // Checking if 6 days have passed (3 + 2 + 1 for unresolved)
                    if (totalDaysElapsed.TotalDays >= 6)
                    {
                        newReputation = await DecreaseLeaderReputationForApprovalDelayAsync(leader);

                        // Create and save the new status for the current complaint
                        var newComplaintStatus = new ComplaintStatus
                        {
                            ComplaintId = complaint.ComplaintId,
                            Status = (int)Status.Unresolved,
                            AttemptNumber = complaint.AttemptNumber,
                            OpenedDate = complaint.OpenedDate,
                            DeadlineDate = complaint.DeadlineDate,
                            NewOpenedDate = currentTime,
                            DateCreated = DateTime.UtcNow,
                            DateUpdated = DateTime.UtcNow
                        };
                        await _complaintStatusRepository.AddComplaintStatusAsync(newComplaintStatus);

                        // notify leader and citizen about complaint unResolved

                        var citizen = await _userRepository.GetUserByIdAsync(complaintRecord.CitizenId);

                        var leaderUnResolvedTemplate = await _emailTemplateRepository.GetByNameAsync(Constants.EmailTemplates.ComplaintLeaderUnresolved);
                        var citizenUnResolvedTemplate = await _emailTemplateRepository.GetByNameAsync(Constants.EmailTemplates.ComplaintCitizenUnresolved);

                        var leaderUnResolvedPayload = new Dictionary<string, string>
                        {
                          {"LeaderName",leader.FullName},
                          {"ComplaintTitle",complaintRecord.Title },
                          { "PreviousReputation", leader.Reputation.ToString()},
                          { "NewReputation", newReputation.ToString() }
                        };

                        var citizenUnResolvedPayload = new Dictionary<string, string>
                        {
                          {"CitizenName",citizen!.FullName},
                          {"ComplaintTitle",complaintRecord.Title }

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
                    // Checking if 5 days have passed (3 + 2)
                    else if (totalDaysElapsed.TotalDays >= 5)
                    {
                        complaint.NewOpenedDate = currentTime; // Update NewOpenedDate to current time
                        newReputation = await DecreaseLeaderReputationForApprovalDelayAsync(leader);
                        await _complaintStatusRepository.UpdateComplaintStatus(complaint);

                        // Notify Leader to approve the backlog complaint , with new open date and his reputation

                        var leaderApprovalTemplate = await _emailTemplateRepository.GetByNameAsync(Constants.EmailTemplates.ComplaintLeaderApprovalDelay);

                        var leaderApprovalPayload = new Dictionary<string, string>
                        {
                          {"LeaderName",leader.FullName},
                          {"ComplaintTitle",complaintRecord.Title },
                          { "PreviousReputation", leader.Reputation.ToString()},
                          { "NewReputation", newReputation.ToString()}

                        };

                        var leaderApprovalDelayEmailVm = new EmailVm
                        {
                            ToUserEmail = leader.Email,
                            TemplateId = leaderApprovalTemplate.Id,
                            Payload = leaderApprovalPayload
                        };

                        await _emailService.SendEmailAsync(leaderApprovalDelayEmailVm);

                    }
                }
            }
        }
    }

    private async Task<double> DecreaseLeaderReputationForApprovalDelayAsync(User leader)
    {
        leader.Reputation -= 10;
        if (leader.Reputation < 0)
            leader.Reputation = 0;

        await _userRepository.UpdateLeaderReputationAsync(leader.UserId, leader.Reputation);
        if (leader.Reputation < 15)
        {
            //await TriggerReElectionAsync(leader)
        }
        return leader.Reputation;
    }

    public async Task<bool> DeleteComplaintByComplaintIdAsync(int complaintId)
    {
        var complaint = await _complaintRepository.GetComplaintByComplaintIdAsync(complaintId);
        var userId = await _claimsService.GetUserIdFromClaimsAsync();
        if (complaint == null)
            return false;

        if (complaint.CitizenId != userId)
            throw new WelfareWorkTrackerException("you cannot delete", (int)HttpStatusCode.Unauthorized); // who raised the complaint only can delete

        var complaintStatus = await _complaintStatusRepository.GetComplaintStatusAsync(complaintId)
            ?? throw new WelfareWorkTrackerException("No complaint status found");
        if (complaintStatus.Status != 1)
            throw new WelfareWorkTrackerException("you cannot delete"); // as complaint is already validated by admin rep

        // To delete a record/complaint from complaintStatus table as there will no other connections with other tables(comments..) as it is underValidating stage
        await _complaintStatusRepository.DeleteComplaintStatusByComplaintIdAsync(complaintId);

        // To delete the images related to that complaint
        var existingImages = await _complaintImageRepository.GetAllComplaintImagesByComplaintIdAsync(complaintId);
        foreach (var image in existingImages)
        {
            await _complaintImageRepository.RemoveComplaintImageAsync(image);
        }

        await _complaintRepository.DeleteComplaintByComplaintAsync(complaint);

        return true;
    }

    public async Task<ComplaintDto?> GetComplaintByComplaintIdAsync(int complaintId)
    {
        var complaint = await _complaintRepository.GetComplaintByComplaintIdAsync(complaintId);
        var complaintImages = await _complaintImageRepository.GetAllComplaintImagesByComplaintIdAsync(complaintId);
        var complaintStatus = await _complaintStatusRepository.GetComplaintStatusAsync(complaintId);
        var imageUrls = complaintImages.Select(image => image.ImageUrl).ToList();
        if (complaint == null)
            return null;

        var complaintDto = _mapper.Map<ComplaintDto>(complaint);
        complaintDto.Images = imageUrls;
        complaintDto.Status = ((Status)complaintStatus!.Status).ToString();

        return complaintDto;


    }

    public async Task<List<ComplaintDto>> GetComplaintsAsync()
    {
        var complaints = await _complaintRepository.GetComplaintsAsync();
        var complaintDtos = new List<ComplaintDto>();
        foreach (var complaint in complaints)
        {
            var complaintImages = await _complaintImageRepository.GetAllComplaintImagesByComplaintIdAsync(complaint.ComplaintId);
            var complaintStatus = await _complaintStatusRepository.GetComplaintStatusAsync(complaint.ComplaintId);
            var imageUrls = complaintImages.Select(image => image.ImageUrl).ToList();
            var complaintDto = _mapper.Map<ComplaintDto>(complaint);
            complaintDto.Images = imageUrls;
            complaintDto.Status = ((Status)complaintStatus!.Status).ToString();
            complaintDtos.Add(complaintDto);
        }
        return complaintDtos;
    }

    public async Task<List<ComplaintDto>> GetComplaintsByUserIdAsync(int userId)
    {
        var complaints = await _complaintRepository.GetComplaintsByUserIdAsync(userId);
        var complaintDtos = new List<ComplaintDto>();
        foreach (var complaint in complaints)
        {
            var complaintImages = await _complaintImageRepository.GetAllComplaintImagesByComplaintIdAsync(complaint.ComplaintId);
            var complaintStatus = await _complaintStatusRepository.GetComplaintStatusAsync(complaint.ComplaintId);
            var imageUrls = complaintImages.Select(image => image.ImageUrl).ToList();
            var complaintDto = _mapper.Map<ComplaintDto>(complaint);
            complaintDto.Images = imageUrls;
            complaintDto.Status = ((Status)complaintStatus!.Status).ToString();
            complaintDtos.Add(complaintDto);
        }
        return complaintDtos;

    }

    public async Task<List<ComplaintDto>> GetComplaintsForAdminRepAsync(string constituencyName)
    {
        var complaints = await _complaintRepository.GetComplaintsByConstituency(constituencyName);
        var complaintDtos = new List<ComplaintDto>();
        foreach (var complaint in complaints)
        {
            var complaintImages = await _complaintImageRepository.GetAllComplaintImagesByComplaintIdAsync(complaint.ComplaintId);
            var complaintStatus = await _complaintStatusRepository.GetComplaintStatusAsync(complaint.ComplaintId);
            var imageUrls = complaintImages.Select(image => image.ImageUrl).ToList();
            var complaintDto = _mapper.Map<ComplaintDto>(complaint);
            complaintDto.Images = imageUrls;
            complaintDto.Status = ((Status)complaintStatus!.Status).ToString();
            complaintDtos.Add(complaintDto);
        }
        return complaintDtos;
    }

    public async Task<List<ComplaintDto>?> GetComplaintsForLeaderByLeaderIdAsync(int leaderId, Status status)
    {
        var complaints = await _complaintRepository.GetComplaintsForLeaderByLeaderIdAsync(leaderId);

        if (complaints == null)
            return null;

        var complaintDtos = new List<ComplaintDto>();

        foreach (var complaint in complaints)
        {
            var complaintStatus = await _complaintStatusRepository.GetComplaintStatusAsync(complaint.ComplaintId);

            if (complaintStatus!.Status == (int)status)
            {
                var complaintImages = await _complaintImageRepository.GetAllComplaintImagesByComplaintIdAsync(complaint.ComplaintId);
                var imageUrls = complaintImages.Select(image => image.ImageUrl).ToList();
                var complaintDto = _mapper.Map<ComplaintDto>(complaint);
                complaintDto.Images = imageUrls;
                complaintDto.Status = ((Status)complaintStatus.Status).ToString();
                complaintDtos.Add(complaintDto);
            }
        }
        return complaintDtos;
    }

    public async Task<List<ComplaintDto>> GetRecentComplaintsAsync(string constituencyName)
    {
        var complaints = await _complaintRepository.GetRecentComplaintsAsync(constituencyName);
        var complaintDtos = new List<ComplaintDto>();

        foreach (var complaint in complaints)
        {
            var images = await _complaintImageRepository.GetAllComplaintImagesByComplaintIdAsync(complaint.ComplaintId);
            var imageUrls = images.Select(i => i.ImageUrl).ToList();
            var complaintStatus = await _complaintStatusRepository.GetComplaintStatusAsync(complaint.ComplaintId) 
                ?? throw new WelfareWorkTrackerException("Complaint Status is null.");

            var complaintDto = _mapper.Map<ComplaintDto>(complaint);
            complaintDto.Images = imageUrls;
            complaintDto.Status = ((Status)complaintStatus.Status).ToString();

            complaintDtos.Add(complaintDto);
        }

        return complaintDtos;
    }

    public async Task<ComplaintDto?> UpdateComplaintByComplaintIdAsync(int complaintId, ComplaintVm complaintVm)
    {
        var complaint = await _complaintRepository.GetComplaintByComplaintIdAsync(complaintId);
        var userId = await _claimsService.GetUserIdFromClaimsAsync();
        if (complaint == null)
            return null;

        if (complaint.CitizenId != userId)
            throw new WelfareWorkTrackerException("You cannot update this complaint", (int)HttpStatusCode.Unauthorized); // users who raises the complaint can update

        var complaintStatus = await _complaintStatusRepository.GetComplaintStatusAsync(complaintId);

        if (complaintStatus!.Status != (int)Status.UnderValidation)
            throw new WelfareWorkTrackerException("This complaint cannot be edited at this stage"); // Citizen cannot update as the status is changed from UnderValidation stage

        var leader = await _userRepository.GetLeaderByConstituencyIdAsync(complaint.ConstituencyId);

        complaint.CitizenId = userId;
        complaint.Title = complaintVm.Title;
        complaint.Description = complaintVm.Description;
        complaint.ConstituencyName = complaintVm.ConstituencyName;
        complaint.LeaderId = leader!.UserId;
        complaint.DateUpdated = DateTime.UtcNow;

        // Update images
        var newImageUrls = complaintVm.Images ?? [];

        // Deletes existing images to update with new images
        var existingImages = await _complaintImageRepository.GetAllComplaintImagesByComplaintIdAsync(complaintId);
        foreach (var image in existingImages)
        {
            await _complaintImageRepository.RemoveComplaintImageAsync(image);
        }

        // Add new images
        foreach (var imageUrl in newImageUrls)
        {
            var newImage = new ComplaintImage
            {
                ComplaintId = complaintId,
                ImageUrl = imageUrl
            };
            await _complaintImageRepository.AddComplaintImageAsync(newImage);
        }

        // Retrieve updated image URLs
        var updatedImages = await _complaintImageRepository.GetAllComplaintImagesByComplaintIdAsync(complaintId);
        var imageUrls = updatedImages.Select(image => image.ImageUrl).ToList();

        await _complaintRepository.UpdateComplaintByIdAsync(complaint);

        var complaintDto = _mapper.Map<ComplaintDto>(complaint);
        complaintDto.Images = imageUrls;
        complaintDto.Status = ((Status)complaintStatus.Status).ToString();

        return complaintDto;

    }
}
