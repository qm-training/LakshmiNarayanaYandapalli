namespace WelfareWorkTracker.Core.Contracts.Service;
public interface ICommentService
{
    Task<List<CommentDto>> GetCommentsByIdAsync(int? complaintId, int? dailyComplaintId);
    Task<CommentDto> UpdateCommentByIdAsync(int commentId, CommentVm commentVm);
    Task<CommentDto?> AddCommentAsync(int? complaintId, int? dailyComplaintId, CommentVm commentVm);
    Task<bool> DeleteCommentByCommentIdAsync(int commentId);
}
