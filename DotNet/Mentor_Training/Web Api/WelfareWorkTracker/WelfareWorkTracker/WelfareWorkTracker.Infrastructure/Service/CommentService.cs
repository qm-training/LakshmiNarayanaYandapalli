namespace WelfareWorkTracker.Infrastructure.Service;
public class CommentService(ICommentRepository commentRepository,
                                IComplaintRepository complaintRepository,
                                IDailyComplaintRepository dailyComplaintRepository,
                                IClaimsService claimsService,
                                IMapper mapper) : ICommentService
{
    private readonly ICommentRepository _commentRepository = commentRepository;
    private readonly IComplaintRepository _complaintRepository = complaintRepository;
    private readonly IDailyComplaintRepository _dailyComplaintRepository = dailyComplaintRepository;
    private readonly IClaimsService _claimsService = claimsService;
    private readonly IMapper _mapper = mapper;

    public async Task<CommentDto?> AddCommentAsync(int? complaintId, int? dailyComplaintId, CommentVm commentVm)
    {
        var userId = await _claimsService.GetUserIdFromClaimsAsync();

        if (!complaintId.HasValue && !dailyComplaintId.HasValue)
            throw new WelfareWorkTrackerException("Any one of the ID's should be provided.", (int)HttpStatusCode.BadRequest);

        else if (complaintId.HasValue && dailyComplaintId.HasValue)
            throw new WelfareWorkTrackerException("only one of the ID's should be provided.", (int)HttpStatusCode.BadRequest);

        if (complaintId.HasValue)
        {
            var complaint = await _complaintRepository.GetComplaintByComplaintIdAsync((int)complaintId);
            if (complaint == null)
                return null;

            var comment = new Comment
            {
                Description = commentVm.Description,
                UserId = userId,
                ComplaintId = complaintId,
                DailyComplaintId = dailyComplaintId,
                DateCreated = DateTime.UtcNow,
                IsAnonymous = commentVm.IsAnonymous,
            };
            var addComment = await _commentRepository.AddCommentByIdAsync(comment);
            
            return _mapper.Map<CommentDto>(addComment);
        }
        else if (dailyComplaintId.HasValue)
        {
            var dailyComplaint = await _dailyComplaintRepository.GetDailyComplaintByIdAsync((int)dailyComplaintId);
            if (dailyComplaint == null)
                return null;

            var comment = new Comment
            {
                Description = commentVm.Description,
                UserId = userId,
                ComplaintId = complaintId,
                DailyComplaintId = dailyComplaintId,
                DateCreated = DateTime.UtcNow,
                IsAnonymous = commentVm.IsAnonymous,
            };
            var addComment = await _commentRepository.AddCommentByIdAsync(comment);
            
            return _mapper.Map<CommentDto>(addComment);
        }
        return new CommentDto();
    }

    public async Task<List<CommentDto>> GetCommentsByIdAsync(int? complaintId, int? dailyComplaintId)
    {
        if (!complaintId.HasValue && !dailyComplaintId.HasValue)
            throw new WelfareWorkTrackerException("Any one of the ID's should be provided.", (int)HttpStatusCode.BadRequest);

        else if (complaintId.HasValue && dailyComplaintId.HasValue)
            throw new WelfareWorkTrackerException("only one of the ID's should be provided.", (int)HttpStatusCode.BadRequest);

        var userId = await _claimsService.GetUserIdFromClaimsAsync();
        var comments = await _commentRepository.GetCommentsByIdAsync(complaintId, dailyComplaintId, userId);
        
        return _mapper.Map<List<CommentDto>>(comments);
    }

    public async Task<CommentDto> UpdateCommentByIdAsync(int commentId, CommentVm commentVm)
    {
        // Get the current user ID from claims
        var userId = await _claimsService.GetUserIdFromClaimsAsync();

        // Retrieve the existing comment or throw an exception if not found
        var existingComment = await _commentRepository.GetCommentByIdAsync(commentId) ??
            throw new WelfareWorkTrackerException("comment not found", (int)HttpStatusCode.NotFound);

        // Ensure the comment belongs to the logged-in user
        if (existingComment.UserId != userId)
            throw new WelfareWorkTrackerException("you cannot update", (int)HttpStatusCode.Unauthorized);

        // Update the comment with new values
        existingComment.IsAnonymous = commentVm.IsAnonymous;
        existingComment.Description = commentVm.Description;
        existingComment.DateUpdated = DateTime.UtcNow;

        // Save the updated comment
        var updatedComment = await _commentRepository.UpdateCommentByIdAsync(existingComment);

        return _mapper.Map<CommentDto>(updatedComment);
    }

    public async Task<bool> DeleteCommentByCommentIdAsync(int commentId)
    {
        var userId = await _claimsService.GetUserIdFromClaimsAsync();

        var comment = await _commentRepository.GetCommentByIdAsync(commentId) 
                ?? throw new WelfareWorkTrackerException("comment not found", (int)HttpStatusCode.NotFound);

        if (comment.UserId != userId)
            throw new WelfareWorkTrackerException("you cannot delete", (int)HttpStatusCode.Unauthorized);

        var deletedComment = await _commentRepository.DeleteCommentAsync(comment);

        if (deletedComment == null)
            return false;

        return true;
    }
}
