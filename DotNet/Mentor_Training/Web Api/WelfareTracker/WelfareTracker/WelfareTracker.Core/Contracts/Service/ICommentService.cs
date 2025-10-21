using WelfareTracker.Core.Dtos;
using WelfareTracker.Core.Vms;

namespace WelfareTracker.Core.Contracts.Service
{
    public interface ICommentService
    {
        Task<CommentDto> AddComment(CommentVm commentVm);
        Task<CommentDto> UpdateCommentByIdAsync(int commentId, CommentVm commentVm);
        Task<bool> DeleteCommentByIdAsync(int commentId);
        Task<List<CommentDto>> GetCommentsByCitizenId(int citizenId);
        Task<List<CommentDto>> GetCommentsByComplaintId(int complaintId);

    }
}
