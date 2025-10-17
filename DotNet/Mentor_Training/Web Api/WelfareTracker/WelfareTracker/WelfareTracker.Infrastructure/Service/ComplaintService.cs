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
    public class ComplaintService(IComplaintRepository complaintRepository, 
                                    IComplaintStatusRepository complaintStatusRepository, 
                                    IComplaintImageRepository complaintImageRepository,
                                    IUserRepository userRepository,
                                    IClaimsService claimsService,
                                    IMapper mapper) : IComplaintService
    {
        private readonly IComplaintRepository _complaintRepository = complaintRepository;
        private readonly IComplaintStatusRepository _complaintStatusRepository = complaintStatusRepository;
        private readonly IComplaintImageRepository _complaintImageRepository = complaintImageRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IClaimsService _claimsService = claimsService;
        private readonly IMapper _mapper = mapper;

        public async Task<ComplaintDto> AddComplaintAsync(ComplaintVm complaintVm)
        {
            var consituencyName = await _claimsService.GetConstituencyNameFromClaimsAsync();
            var userId = await _claimsService.GetUserIdFromClaimsAsync();

            var leader = await _userRepository.GetLeaderByConstituencyName(consituencyName);

            if(complaintVm.ComplaintImageUrls == null || complaintVm.ComplaintImageUrls.Count == 0)
            {
                throw new WelfareTrackerException("Add atleast one image");
            }

            var complaint = new Complaint
            {
                Description = complaintVm.Description,
                ConstituencyId = leader!.ConstituencyId,
                CitizenId = userId,
                LeaderId = leader!.UserId,
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
                Status = (int) Status.UnderValidation,
                AttemptNumber = createdComplaint.Attempts,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            };

            await _complaintStatusRepository.AddComplaintStatusAsync(complaintStatus);

            var complaintDto = _mapper.Map<ComplaintDto>(createdComplaint);
            complaintDto.Status = ((Status)complaintStatus.Status).ToString();

            return complaintDto;
        }

        public async Task<ComplaintDto> GetComplaintByComplaintIdAsync(int complaintId)
        {
            var complaint = await _complaintRepository.GetComplaintByIdAsync(complaintId);
            if (complaint == null)
            {
                throw new WelfareTrackerException("Complaint not found", (int)HttpStatusCode.NotFound);
            }
            var complaintStatus = await _complaintStatusRepository.GetComplaintStatusByIdAsync(complaintId);

            var complaintDto = _mapper.Map<ComplaintDto>(complaint);
            complaintDto.Status = ((Status)complaintStatus!.Status).ToString();

            return complaintDto;
        }

        public async Task<List<ComplaintDto>?> GetComplaintsByCitizenIdAsync(int citizenId)
        {
            var complaints = await _complaintRepository.GetComplaintsByCitizenIdAsync(citizenId);
            if (complaints == null || complaints.Count == 0)
            {
                return null;
            }
            var complaintDtos = new List<ComplaintDto>();
            foreach (var complaint in complaints)
            {
                var complaintStatus = await _complaintStatusRepository.GetComplaintStatusByIdAsync(complaint.ComplaintId);
                var complaintDto = _mapper.Map<ComplaintDto>(complaint);
                complaintDto.Status = ((Status)complaintStatus!.Status).ToString();
                complaintDtos.Add(complaintDto);
            }
            return complaintDtos;
        }

        public async Task<ComplaintDto?> UpdateComplaintAsync(int complaintId, ComplaintVm complaintVm)
        {
            var userId = await _claimsService.GetUserIdFromClaimsAsync();
            var complaint = await _complaintRepository.GetComplaintByIdAsync(complaintId);
            
            if (complaint == null)
            {
                return null;
            }

            if (complaint.CitizenId != userId)
            {
                throw new WelfareTrackerException("You are not authorized to update this complaint.");
            }

            var complaintStatus = await _complaintStatusRepository.GetComplaintStatusByIdAsync(complaintId);

            if(complaintStatus!.Status != (int)Status.UnderValidation)
            {
                throw new WelfareTrackerException("Complaint cannot be updated", (int)HttpStatusCode.Unauthorized);
            }

            complaint.Description = complaintVm.Description;
            complaint.DateUpdated = DateTime.UtcNow;

            var existingImages = await _complaintImageRepository.GetComplaintImagesByComplaintIdAsync(complaintId);
            foreach (var image in existingImages)
            {
                await _complaintImageRepository.DeleteComplaintImageAsync(image);
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
            complaintDto.Status = ((Status)complaintStatus.Status).ToString();
            return complaintDto;
        }
    }
}
