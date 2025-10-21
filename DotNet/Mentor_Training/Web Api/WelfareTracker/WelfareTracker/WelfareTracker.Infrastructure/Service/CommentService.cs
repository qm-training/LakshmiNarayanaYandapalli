using AutoMapper;
using System.Net;
using WelfareTracker.Core.Contracts.Repository;
using WelfareTracker.Core.Contracts.Service;
using WelfareTracker.Core.Dtos;
using WelfareTracker.Core.Exceptions;
using WelfareTracker.Core.Models;
using WelfareTracker.Core.Vms;

namespace WelfareTracker.Infrastructure.Service
{
    public class CommentService(ICommentRepository commentRepository,
                                    IComplaintRepository complaintRepository,
                                    IClaimsService claimsService,
                                    IMapper mapper) : ICommentService
    {
        private readonly ICommentRepository _commentRepository = commentRepository;
        private readonly IComplaintRepository _complaintRepository = complaintRepository;
        private readonly IClaimsService _claimsService = claimsService;
        private readonly IMapper _mapper = mapper;
        public async Task<CommentDto> AddComment(CommentVm commentVm)
        {
            var userId = await _claimsService.GetUserIdFromClaimsAsync();

            if (!commentVm.ComplaintId.HasValue && !commentVm.DailyComplaintId.HasValue)
                throw new WelfareTrackerException("Any one of the ID's should be provided.", (int)HttpStatusCode.BadRequest);

            else if (commentVm.ComplaintId.HasValue && commentVm.DailyComplaintId.HasValue)
                throw new WelfareTrackerException("only one of the ID's should be provided.", (int)HttpStatusCode.BadRequest);

            var complaint = await _complaintRepository.GetComplaintByIdAsync(commentVm.ComplaintId!.Value) ?? throw new WelfareTrackerException("Complaint not found.");
            var complaintId = complaint.ReferenceNumber != 0 ? complaint.ReferenceNumber : complaint.ComplaintId;
            if (commentVm.ComplaintId != 0)
            {
                var comment = new Comment
                {
                    Description = commentVm.Description,
                    UserId = userId,
                    ComplaintId = complaintId,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,
                    IsAnonymous = commentVm.IsAnonymous
                };

                var addedcomment = await _commentRepository.AddCommentAsync(comment);

                var mapperDto = _mapper.Map<CommentDto>(addedcomment);
                return mapperDto;
            }
            return null;
        }

        public async Task<bool> DeleteCommentByIdAsync(int commentId)
        {
            var userId = await _claimsService.GetUserIdFromClaimsAsync();
            var comment = await _commentRepository.GetCommentByCommentIdAsync(commentId) ?? throw new WelfareTrackerException("Comment not found");

            if(userId != comment.UserId)
                throw new WelfareTrackerException("you cannot update", (int)HttpStatusCode.Unauthorized);

            return await _commentRepository.DeleteCommentAsync(comment);
        }

        public async Task<List<CommentDto>> GetCommentsByCitizenId(int citizenId)
        {
            var userId = await _claimsService.GetUserIdFromClaimsAsync();
            if (userId != citizenId)
                throw new WelfareTrackerException("Cannot access other user comments", (int)HttpStatusCode.Unauthorized);

            var comments = await _commentRepository.GetCommentsByUserIdAsync(citizenId);
            if(comments == null || comments.Count == 0)
            {
                throw new WelfareTrackerException("No comments found for citizen.");
            }
            var commentDtos = new List<CommentDto>();
            foreach (var comment in comments)
            {
                var commentDto = _mapper.Map<CommentDto>(comment);
                commentDtos.Add(commentDto);
            }
            return commentDtos;
        }

        public async Task<List<CommentDto>> GetCommentsByComplaintId(int complaintId)
        {
            var complaint = await _complaintRepository.GetComplaintByIdAsync(complaintId) ?? throw new WelfareTrackerException("Complaint not found.");

            var newComplaintId = complaint.ReferenceNumber != 0 ? complaint.ReferenceNumber : complaintId;
            var comments = await _commentRepository.GetCommentsByComplaintIdAsync(newComplaintId);
            if(comments == null || comments.Count == 0)
            {
                throw new WelfareTrackerException("No Comments found for the complaint");
            }

            var commentDtos = new List<CommentDto>();

            foreach (var comment in comments)
            {
                var commentDto = new CommentDto
                {
                    CommentId = comment.CommentId,
                    Description = comment.Description,
                    UserId = comment.IsAnonymous ? comment.UserId : 0,
                    ComplaintId = comment.ComplaintId,
                    DailyComplaintId = comment.DailyComplaintId,
                    DateCreated = comment.DateCreated,
                    DateUpdated = comment.DateUpdated
                };
                commentDtos.Add(commentDto);
            }
            return commentDtos;
        }

        public async Task<CommentDto> UpdateCommentByIdAsync(int commentId, CommentVm commentVm)
        {
            if (!commentVm.ComplaintId.HasValue && !commentVm.DailyComplaintId.HasValue)
                throw new WelfareTrackerException("Any one of the ID's should be provided.", (int)HttpStatusCode.BadRequest);

            else if (commentVm.ComplaintId.HasValue && commentVm.DailyComplaintId.HasValue)
                throw new WelfareTrackerException("only one of the ID's should be provided.", (int)HttpStatusCode.BadRequest);

            var userId = await _claimsService.GetUserIdFromClaimsAsync();
            var comment = await _commentRepository.GetCommentByCommentIdAsync(commentId) ?? throw new WelfareTrackerException("comment not found", (int)HttpStatusCode.NotFound);

            if(userId != comment.UserId)
                throw new WelfareTrackerException("you cannot update", (int)HttpStatusCode.Unauthorized);

            comment.DateUpdated = DateTime.UtcNow;
            comment.Description = commentVm.Description;
            comment.IsAnonymous = commentVm.IsAnonymous;

            var updatedComment = await _commentRepository.UpdateCommentAsync(comment);
            var commentDto = _mapper.Map<CommentDto>(updatedComment);
            return commentDto;
        }
    }
}
