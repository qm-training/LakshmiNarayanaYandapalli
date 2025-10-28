using AutoMapper;
using System.Net;
using WelfareWorkTracker.Core.Contracts.Repository;
using WelfareWorkTracker.Core.Contracts.Service;
using WelfareWorkTracker.Core.Dtos;
using WelfareWorkTracker.Core.Exceptions;
using WelfareWorkTracker.Core.Models;
using WelfareWorkTracker.Core.Vms;

namespace WelfareWorkTracker.Infrastructure.Service
{
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
        public async Task<CommentDto> AddComment(CommentVm commentVm)
        {
            var userId = await _claimsService.GetUserIdFromClaimsAsync();

            if (!commentVm.ComplaintId.HasValue && !commentVm.DailyComplaintId.HasValue)
                throw new WelfareWorkTrackerException("Any one of the ID's should be provided.", (int)HttpStatusCode.BadRequest);

            else if (commentVm.ComplaintId.HasValue && commentVm.DailyComplaintId.HasValue)
                throw new WelfareWorkTrackerException("only one of the ID's should be provided.", (int)HttpStatusCode.BadRequest);

            if (commentVm.ComplaintId != 0)
            {
                var complaint = await _complaintRepository.GetComplaintByComplaintIdAsync(commentVm.ComplaintId!.Value)
                                                ?? throw new WelfareWorkTrackerException("Complaint not found.", (int)HttpStatusCode.NotFound);
                var complaintId = complaint.ReferenceNumber != 0 ? complaint.ReferenceNumber : complaint.ComplaintId;

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
            else
            {
                var dailyComplaint = await _dailyComplaintRepository.GetDailyComplaintByIdAsync(commentVm.DailyComplaintId!.Value) 
                                                ?? throw new WelfareWorkTrackerException("Daily Complaint not found.", (int)HttpStatusCode.NotFound);
                var comment = new Comment
                {
                    Description = commentVm.Description,
                    UserId = userId,
                    DailyComplaintId = dailyComplaint.DailyComplaintId,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,
                    IsAnonymous = commentVm.IsAnonymous
                };

                var addedcomment = await _commentRepository.AddCommentAsync(comment);

                var mapperDto = _mapper.Map<CommentDto>(addedcomment);
                return mapperDto;
            }
        }

        public async Task<bool> DeleteCommentByIdAsync(int commentId)
        {
            var userId = await _claimsService.GetUserIdFromClaimsAsync();
            var comment = await _commentRepository.GetCommentByCommentIdAsync(commentId) 
                                        ?? throw new WelfareWorkTrackerException("Comment not found", (int)HttpStatusCode.NotFound);

            if (userId != comment.UserId)
                throw new WelfareWorkTrackerException("you cannot update", (int)HttpStatusCode.Unauthorized);

            return await _commentRepository.DeleteCommentAsync(comment);
        }

        public async Task<List<CommentDto>> GetCommentsByCitizenId(int citizenId)
        {
            var userId = await _claimsService.GetUserIdFromClaimsAsync();
            if (userId != citizenId)
                throw new WelfareWorkTrackerException("Cannot access other user comments", (int)HttpStatusCode.Unauthorized);

            var comments = await _commentRepository.GetCommentsByUserIdAsync(citizenId);
            if (comments == null || comments.Count == 0)
            {
                throw new WelfareWorkTrackerException("No comments found for citizen.", (int)HttpStatusCode.NotFound);
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
            var complaint = await _complaintRepository.GetComplaintByComplaintIdAsync(complaintId) 
                                        ?? throw new WelfareWorkTrackerException("Complaint not found.", (int)HttpStatusCode.NotFound);

            var newComplaintId = complaint.ReferenceNumber != 0 ? complaint.ReferenceNumber : complaintId;
            var comments = await _commentRepository.GetCommentsByComplaintIdAsync(newComplaintId);
            if (comments == null || comments.Count == 0)
            {
                throw new WelfareWorkTrackerException("No Comments found for the complaint", (int)HttpStatusCode.NotFound);
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
                throw new WelfareWorkTrackerException("Any one of the ID's should be provided.", (int)HttpStatusCode.BadRequest);

            else if (commentVm.ComplaintId.HasValue && commentVm.DailyComplaintId.HasValue)
                throw new WelfareWorkTrackerException("only one of the ID's should be provided.", (int)HttpStatusCode.BadRequest);

            var userId = await _claimsService.GetUserIdFromClaimsAsync();
            var comment = await _commentRepository.GetCommentByCommentIdAsync(commentId) 
                                    ?? throw new WelfareWorkTrackerException("comment not found", (int)HttpStatusCode.NotFound);

            if (userId != comment.UserId)
                throw new WelfareWorkTrackerException("you cannot update", (int)HttpStatusCode.Unauthorized);

            comment.DateUpdated = DateTime.UtcNow;
            comment.Description = commentVm.Description;
            comment.IsAnonymous = commentVm.IsAnonymous;

            var updatedComment = await _commentRepository.UpdateCommentAsync(comment);
            var commentDto = _mapper.Map<CommentDto>(updatedComment);
            return commentDto;
        }
    }
}
