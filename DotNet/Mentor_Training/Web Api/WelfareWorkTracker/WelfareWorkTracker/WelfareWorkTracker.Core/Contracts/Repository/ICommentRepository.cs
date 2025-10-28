using WelfareWorkTracker.Core.Models;

namespace WelfareWorkTracker.Core.Contracts.Repository
{
    public interface ICommentRepository
    {
        Task<Comment> AddCommentAsync(Comment comment);
        Task<Comment> UpdateCommentAsync(Comment comment);
        Task<bool> DeleteCommentAsync(Comment comment);
        Task<List<Comment>> GetCommentsByComplaintIdAsync(int complaintId);
        Task<List<Comment>> GetCommentsByUserIdAsync(int userId);
        Task<Comment?> GetCommentByCommentIdAsync(int commentId);
    }
}
