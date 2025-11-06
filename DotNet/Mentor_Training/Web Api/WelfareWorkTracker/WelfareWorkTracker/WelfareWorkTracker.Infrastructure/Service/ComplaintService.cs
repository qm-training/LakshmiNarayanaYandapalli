using AutoMapper;
using System.Net;
using WelfareWorkTracker.Core.Constants;
using WelfareWorkTracker.Core.Contracts.Repository;
using WelfareWorkTracker.Core.Contracts.Service;
using WelfareWorkTracker.Core.Dtos;
using WelfareWorkTracker.Core.Enums;
using WelfareWorkTracker.Core.Exceptions;
using WelfareWorkTracker.Core.Models;
using WelfareWorkTracker.Core.Vms;
using WelfareWorkTracker.Infrastructure.Repository;

namespace WelfareWorkTracker.Infrastructure.Service
{
    public class ComplaintService(IComplaintRepository complaintRepository,
                                    IComplaintStatusRepository complaintStatusRepository,
                                    IComplaintImageRepository complaintImageRepository,
                                    IUserRepository userRepository,
                                    IConstituencyRepository constituencyRepository,
                                    IEmailTemplateRepository emailTemplateRepository,
                                    IClaimsService claimsService,
                                    IEmailService emailService,
                                    IMapper mapper) : IComplaintService
    {
        private readonly IComplaintRepository _complaintRepository = complaintRepository;
        private readonly IComplaintStatusRepository _complaintStatusRepository = complaintStatusRepository;
        private readonly IComplaintImageRepository _complaintImageRepository = complaintImageRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IConstituencyRepository _constituencyRepository = constituencyRepository;
        private readonly IEmailTemplateRepository _emailTemplateRepository = emailTemplateRepository;
        private readonly IClaimsService _claimsService = claimsService;
        private readonly IEmailService _emailService = emailService;
        private readonly IMapper _mapper = mapper;
        public async Task<ComplaintDto> AddComplaintAsync(ComplaintVm complaintVm)
        {
            var consituencyName = await _claimsService.GetUserConstituencyFromClaimsAsync();
            var userId = await _claimsService.GetUserIdFromClaimsAsync();

            var leader = await _userRepository.GetLeaderByConstituencyNameAsync(consituencyName);

            if (complaintVm.ComplaintImageUrls == null || complaintVm.ComplaintImageUrls.Count == 0)
            {
                throw new WelfareWorkTrackerException("Add atleast one image");
            }

            var complaint = new Complaint
            {
                Description = complaintVm.Description,
                ConstituencyId = leader!.ConstituencyId,
                CitizenId = userId,
                LeaderId = leader!.UserId,
                Attempts = 1,
                Status = 1,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            };

            var createdComplaint = await _complaintRepository.AddComplaintAsync(complaint);

            foreach (var imageUrl in complaintVm.ComplaintImageUrls)
            {
                var complaintImage = new ComplaintImage
                {
                    ComplaintId = createdComplaint.ComplaintId,
                    ImageUrl = imageUrl,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow
                };
                await _complaintImageRepository.AddComplaintImageAsync(complaintImage);
            }

            var complaintStatus = new ComplaintStatus
            {
                ComplaintId = createdComplaint.ComplaintId,
                Status = (int)Status.UnderValidation,
                AttemptNumber = createdComplaint.Attempts,
                DateCreated = DateTime.UtcNow,
            };

            await _complaintStatusRepository.AddComplaintStatusAsync(complaintStatus);

            var constituencyName = await _constituencyRepository.GetConstituencyNameByIdAsync(complaint.ComplaintId)
                ?? throw new WelfareWorkTrackerException("Constituency not found.", (int)HttpStatusCode.NotFound);
            var adminRep = await _userRepository.GetAdminRepByConstituencyName(consituencyName)
                ?? throw new WelfareWorkTrackerException("AdminRep not found.", (int)HttpStatusCode.NotFound);

            var emailTemplate = await _emailTemplateRepository.GetByNameAsync(Constants.EmailTemplates.UnderValidation) 
                        ?? throw new WelfareWorkTrackerException($"Email Template Not Found!", (int)HttpStatusCode.NotFound);
            var payload = new Dictionary<string, string> {
            { "userName",adminRep.FullName},
            { "constituencyName",constituencyName},
            };
            var email = new EmailVm
            {
                ToUserEmail = adminRep.Email,
                TemplateId = emailTemplate.Id,
                Payload = payload
            };
            await _emailService.SendEmailAsync(email);

            var complaintDto = _mapper.Map<ComplaintDto>(createdComplaint);

            return complaintDto;
        }

        public Task CheckComplaintsForLeaderApprovalAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteComplaintByIdAsync(int complaintId)
        {
            var userId = await _claimsService.GetUserIdFromClaimsAsync();
            var complaint = await _complaintRepository.GetComplaintByComplaintIdAsync(complaintId);
            if (complaint == null)
            {
                return false;
            }
            if (complaint.CitizenId != userId)
            {
                throw new WelfareWorkTrackerException("You are not authorized to delete this complaint.");
            }
            var complaintStatus = await _complaintStatusRepository.GetComplaintStatusByComplaintIdAsync(complaintId);
            if (complaintStatus!.Status != (int)Status.UnderValidation)
            {
                throw new WelfareWorkTrackerException("Complaint cannot be deleted", (int)HttpStatusCode.Unauthorized);
            }

            var complaintStatusDelete = await _complaintStatusRepository.DeleteComplaintStatusByComplaintIdAsync(complaintId);
            if (!complaintStatusDelete)
            {
                throw new WelfareWorkTrackerException("Error deleting complaint status");
            }

            var complaintImages = await _complaintImageRepository.GetAllComplaintImagesByComplaintIdAsync(complaintId);
            foreach (var image in complaintImages)
            {
                await _complaintImageRepository.RemoveComplaintImageAsync(image);
            }
            await _complaintRepository.DeleteComplaintAsync(complaint);
            return true;
        }

        public async Task<ComplaintDto> GetComplaintByIdAsync(int complaintId)
        {
            var complaint = await _complaintRepository.GetComplaintByComplaintIdAsync(complaintId)
                                        ?? throw new WelfareWorkTrackerException("Complaint not found.", (int)HttpStatusCode.NotFound);

            var complaintDto = _mapper.Map<ComplaintDto>(complaint);
            return complaintDto;
        }

        public async Task<List<ComplaintDto>> GetComplaintsByConstituencyAndStatusAsync(GetComplaintsVm getComplaintsVm)
        {
            var complaints = await _complaintRepository.GetComplaintsForConstituencyAndStatusAsync(getComplaintsVm.Status, getComplaintsVm.ConstituencyId);

            if(complaints == null || complaints.Count == 0)
                throw new WelfareWorkTrackerException("Complaint not found", (int)HttpStatusCode.NotFound);

            var complaintDtos = new List<ComplaintDto>();
            foreach (var complaint in complaints)
            {
                var complaintDto = _mapper.Map<ComplaintDto>(complaint);
                complaintDtos.Add(complaintDto);
            }
            return complaintDtos;
        }

        public async Task<List<ComplaintDto>> GetComplaintsByUserAsync(int userId)
        {
            var complaints = await _complaintRepository.GetComplaintByUserIdAsync(userId);
            if (complaints == null || complaints.Count == 0)
                throw new WelfareWorkTrackerException("Complaint not found.", (int)HttpStatusCode.NotFound);

            var complaintDtos = new List<ComplaintDto>();
            foreach (var complaint in complaints)
            {
                var complaintDto = _mapper.Map<ComplaintDto>(complaint);
                complaintDtos.Add(complaintDto);
            }
            return complaintDtos;
        }

        public async Task<List<ComplaintDto>> GetComplaintsForAdminRepAsync(GetComplaintsVm getComplaintsVm)
        {
            var complaints = await _complaintRepository.GetComplaintsForConstituencyAndStatusAsync(getComplaintsVm.Status, getComplaintsVm.ConstituencyId);

            if (complaints == null || complaints.Count == 0)
                throw new WelfareWorkTrackerException("Complaint not found", (int)HttpStatusCode.NotFound);

            var complaintDtos = new List<ComplaintDto>();
            foreach (var complaint in complaints)
            {
                var complaintDto = _mapper.Map<ComplaintDto>(complaint);
                complaintDtos.Add(complaintDto);
            }
            return complaintDtos;
        }

        public async Task<List<ComplaintDto>> GetComplaintsForLeaderAsync(int leaderId)
        {
            var complaints = await _complaintRepository.GetComplaintsForLeaderAsync(leaderId);
            if (complaints == null || complaints.Count == 0)
                throw new WelfareWorkTrackerException("Complaint not found.", (int)HttpStatusCode.NotFound);

            var complaintDtos = new List<ComplaintDto>();
            foreach (var complaint in complaints)
            {
                var complaintDto = _mapper.Map<ComplaintDto>(complaint);
                complaintDtos.Add(complaintDto);
            }
            return complaintDtos;
        }

        public async Task<List<ComplaintDto>> GetRecentComplaintsInConstituencyAsync(int constituencyId)
        {
            var complaints = await _complaintRepository.GetRecentComplaintsInConstituencyAsync(constituencyId);

            if (complaints == null || complaints.Count == 0)
                throw new WelfareWorkTrackerException("Complaint not found", (int)HttpStatusCode.NotFound);

            var complaintDtos = new List<ComplaintDto>();
            foreach (var complaint in complaints)
            {
                var complaintDto = _mapper.Map<ComplaintDto>(complaint);
                complaintDtos.Add(complaintDto);
            }
            return complaintDtos;
        }

        public async Task<ComplaintDto> UpdateComplaintAsync(int complaintId, ComplaintVm complaintVm)
        {
            var userId = await _claimsService.GetUserIdFromClaimsAsync();
            var complaint = await _complaintRepository.GetComplaintByComplaintIdAsync(complaintId)
                                    ?? throw new WelfareWorkTrackerException("Complaint not found.", (int)HttpStatusCode.NotFound);

            if (complaint.CitizenId != userId)
            {
                throw new WelfareWorkTrackerException("You are not authorized to update this complaint.");
            }

            var complaintStatus = await _complaintStatusRepository.GetComplaintStatusByComplaintIdAsync(complaintId);

            if (complaintStatus!.Status != (int)Status.UnderValidation)
            {
                throw new WelfareWorkTrackerException("Complaint cannot be updated", (int)HttpStatusCode.Unauthorized);
            }

            complaint.Description = complaintVm.Description;
            complaint.DateUpdated = DateTime.UtcNow;

            var existingImages = await _complaintImageRepository.GetAllComplaintImagesByComplaintIdAsync(complaintId);
            foreach (var image in existingImages)
            {
                await _complaintImageRepository.RemoveComplaintImageAsync(image);
            }

            var newImages = complaintVm.ComplaintImageUrls ?? [];
            foreach (var imageUrl in newImages)
            {
                var complaintImage = new ComplaintImage
                {
                    ComplaintId = complaint.ComplaintId,
                    ImageUrl = imageUrl,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow
                };
                await _complaintImageRepository.AddComplaintImageAsync(complaintImage);
            }

            var updatedComplaint = await _complaintRepository.UpdateComplaintAsync(complaint);
            var complaintDto = _mapper.Map<ComplaintDto>(updatedComplaint);
            return complaintDto;
        }

        public async Task<bool> UpdateStatusOfAllReferredComplaintsAsync(int complaintId)
        {
            var complaints = await _complaintRepository.GetAllReferredComplaintsAsync(complaintId);
            var originalComplaint = await _complaintRepository.GetComplaintByComplaintIdAsync(complaintId);
            if (complaints == null || complaints.Count == 0 || originalComplaint == null)
            {
                return false;
            }
            foreach (var complaint in complaints)
            {
                complaint.Status = originalComplaint!.Status;
                complaint.DateUpdated = DateTime.UtcNow;
                await _complaintRepository.UpdateComplaintAsync(complaint);
            }
            return true;
        }
        private async Task DecreaseLeaderReputationForApprovalDelayAsync(User leader)
        {
            leader.Reputation -= 10;
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
