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
    public class ComplaintImageService(IComplaintImageRepository complaintImageRepository,
                                            IClaimsService claimsService,
                                            IComplaintRepository complaintRepository,
                                            IComplaintStatusRepository complaintStatusRepository,
                                            IMapper mapper) : IComplaintImageService
    {
        private readonly IComplaintImageRepository _complaintImageRepository = complaintImageRepository;
        private readonly IClaimsService _claimsService = claimsService;
        private readonly IComplaintRepository _complaintRepository = complaintRepository;
        private readonly IComplaintStatusRepository _complaintStatusRepository = complaintStatusRepository;
        private readonly IMapper _mapper = mapper;
        public async Task<ComplaintImageDto> AddComplaintImageAsync(ComplaintImageVm complaintImageVm)
        {
            var userId = await _claimsService.GetUserIdFromClaimsAsync();
            var complaint = await _complaintRepository.GetComplaintByComplaintIdAsync(complaintImageVm.ComplaintId)
                                ?? throw new WelfareWorkTrackerException("No Complaint found.", (int)HttpStatusCode.NotFound);

            if (userId != complaint.CitizenId)
                throw new WelfareWorkTrackerException("Not Authorized to added image", (int)HttpStatusCode.Unauthorized);

            var complaintImage = new ComplaintImage
            {
                ComplaintId = complaintImageVm.ComplaintId,
                ImageUrl = complaintImageVm.ImageUrl,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            };

            var addedComplaintImage = await _complaintImageRepository.AddComplaintImageAsync(complaintImage);
            var complaintImageDto = _mapper.Map<ComplaintImageDto>(addedComplaintImage);
            return complaintImageDto;
        }

        public async Task<bool> DeleteComplaintImageByIdAsync(int complaintImageId)
        {
            var userId = await _claimsService.GetUserIdFromClaimsAsync();
            var complaintImage = await _complaintImageRepository.GetComplaintImageByIdAsync(complaintImageId)
                                        ?? throw new WelfareWorkTrackerException("No complaint image found.", (int)HttpStatusCode.NotFound);

            var complaint = await _complaintRepository.GetComplaintByComplaintIdAsync(complaintImage.ComplaintId)
                                    ?? throw new WelfareWorkTrackerException("No Complaint found.", (int)HttpStatusCode.NotFound);

            if (complaint.CitizenId != userId)
                throw new WelfareWorkTrackerException("Not Authorized to added image", (int)HttpStatusCode.Unauthorized);

            var complaintStatus = await _complaintStatusRepository.GetComplaintStatusByComplaintIdAsync(complaintImage.ComplaintId);
            if (complaintStatus!.Status != (int)Status.UnderValidation)
                throw new WelfareWorkTrackerException("This complaintImage cannot be delete at this stage");

            return await _complaintImageRepository.RemoveComplaintImageAsync(complaintImage);
        }

        public async Task<ComplaintImageDto> GetComplaintImageByIdAsync(int complaintImageId)
        {
            var complaintImage = await _complaintImageRepository.GetComplaintImageByIdAsync(complaintImageId)
                                            ?? throw new WelfareWorkTrackerException("No complaint image found.", (int)HttpStatusCode.NotFound);

            var complaintImageDto = _mapper.Map<ComplaintImageDto>(complaintImage);
            return complaintImageDto;
        }

        public async Task<List<ComplaintImageDto>> GetComplaintImagesByComplaintIdAsync(int complaintId)
        {
            var complaintImages = await _complaintImageRepository.GetAllComplaintImagesByComplaintIdAsync(complaintId);
            if (complaintImages == null || complaintImages.Count == 0)
                throw new WelfareWorkTrackerException("No complaint image found.", (int)HttpStatusCode.NotFound);

            var complaintImageDtos = new List<ComplaintImageDto>();
            foreach (var image in complaintImages)
            {
                var complaintImageDto = _mapper.Map<ComplaintImageDto>(image);
                complaintImageDtos.Add(complaintImageDto);
            }

            return complaintImageDtos;
        }
    }
}
