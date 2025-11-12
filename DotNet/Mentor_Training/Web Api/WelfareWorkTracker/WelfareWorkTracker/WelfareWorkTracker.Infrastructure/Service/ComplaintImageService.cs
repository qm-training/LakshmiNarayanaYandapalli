namespace WelfareWorkTracker.Infrastructure.Service;
public class ComplaintImageService(IComplaintImageRepository complaintImageRepository,
                                    IComplaintRepository complaintRepository,
                                    IClaimsService claimsService,
                                    IComplaintStatusRepository complaintStatusRepository,
                                    IMapper mapper) : IComplaintImageService
{
    private readonly IComplaintImageRepository _complaintImageRepository = complaintImageRepository;
    private readonly IComplaintRepository _complaintRepository = complaintRepository;
    private readonly IClaimsService _claimsService = claimsService;
    private readonly IComplaintStatusRepository _complaintStatusRepository = complaintStatusRepository;
    private readonly IMapper _mapper = mapper;
    public async Task<ComplaintImageDto?> AddComplaintImageAsync(ComplaintImageVm complaintImageVm)
    {
        var userId = await _claimsService.GetUserIdFromClaimsAsync();
        var complaint = await _complaintRepository.GetComplaintByComplaintIdAsync(complaintImageVm.ComplaintId);
        if (complaint == null)
            return null;

        if (complaint.CitizenId != userId)
            throw new WelfareWorkTrackerException("you cannot add"); // who raised the complaint only can add images

        var complaintImage = new ComplaintImage
        {
            ComplaintId = complaintImageVm.ComplaintId,
            ImageUrl = complaintImageVm.ImageUrl,
            DateCreated = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow,
        };
        var addComplaintImage = await _complaintImageRepository.AddComplaintImageAsync(complaintImage);

        var complaintImageDto = _mapper.Map<ComplaintImageDto>(addComplaintImage);
        return complaintImageDto;

    }

    public async Task<ComplaintImageDto?> GetComplaintImageByIdAsync(int id)
    {
        var complaintImage = await _complaintImageRepository.GetComplaintImageByIdAsync(id);
        if (complaintImage == null)
            return null;
        var complaintImageDto = _mapper.Map<ComplaintImageDto>(complaintImage);
        return complaintImageDto;
    }

    public async Task<List<ComplaintImageDto>?> GetComplaintImagesByComplaintIdAsync(int complaintId)
    {
        var complaintImages = await _complaintImageRepository.GetAllComplaintImagesByComplaintIdAsync(complaintId);
        if (complaintImages == null)
            return null;

        var complaintImageDtos = new List<ComplaintImageDto>();
        foreach (var image in complaintImages)
        {
            var complaintImageDto = _mapper.Map<ComplaintImageDto>(image);
            complaintImageDtos.Add(complaintImageDto);
        }

        return complaintImageDtos;
    }

    public async Task<bool> DeleteComplaintImageByIdAsync(int complaintImageId)
    {
        var userId = await _claimsService.GetUserIdFromClaimsAsync();
        var complaintImage = await _complaintImageRepository.GetComplaintImageByIdAsync(complaintImageId);
        if (complaintImage == null)
            return false;

        var complaint = await _complaintRepository.GetComplaintByComplaintIdAsync(complaintImage!.ComplaintId);
        if (complaint!.CitizenId != userId)
            throw new WelfareWorkTrackerException("you cannot delete"); // who raised the complaint only can delete images

        var complaintStatus = await _complaintStatusRepository.GetComplaintStatusAsync(complaintImage.ComplaintId);
        if (complaintStatus!.Status != (int)Status.UnderValidation)
            throw new WelfareWorkTrackerException("This complaintImage cannot be delete at this stage"); // Citizen cannot delete as the status is changed from UnderValidation stage

        await _complaintImageRepository.RemoveComplaintImageAsync(complaintImage);

        return true;
    }
}