namespace WelfareWorkTracker.Core.Contracts.Repository;
public interface ICommentRepository
{
    Task<Comment> AddCommentByIdAsync(Comment comment);
    Task<List<Comment>> GetCommentsByIdAsync(int? complaintId, int? dailyComplaintId, int userId);
    Task<Comment> UpdateCommentByIdAsync(Comment comment);
    Task<Comment?> GetCommentByIdAsync(int commentId);
    Task<Comment> DeleteCommentAsync(Comment comment);
}
