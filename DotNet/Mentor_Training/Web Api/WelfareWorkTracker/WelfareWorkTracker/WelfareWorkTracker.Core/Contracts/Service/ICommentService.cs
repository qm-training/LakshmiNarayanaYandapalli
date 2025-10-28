using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WelfareWorkTracker.Core.Dtos;
using WelfareWorkTracker.Core.Models;
using WelfareWorkTracker.Core.Vms;

namespace WelfareWorkTracker.Core.Contracts.Service
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
