using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WelfareTracker.Core.Contracts.Repository;
using WelfareTracker.Core.Contracts.Service;
using WelfareTracker.Core.Dtos;
using WelfareTracker.Core.Enums;
using WelfareTracker.Core.Exceptions;
using WelfareTracker.Core.Models;
using WelfareTracker.Core.Vms;
using WelfareTracker.Infrastructure.Repository;

namespace WelfareTracker.Infrastructure.Service
{
    public class FeedbackService(IFeedbackRepository feedbackRepository,
                                    IComplaintRepository complaintRepository,
                                    IDailyComplaintRepository dailyComplaintRepository,
                                    IUserRepository userRepository,
                                    IComplaintStatusRepository complaintStatusRepository,
                                    IDailyComplaintStatusRepository dailyComplaintStatusRepository,
                                    IClaimsService claimsService,
                                    IMapper mapper) : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository = feedbackRepository;
        private readonly IComplaintRepository _complaintRepository = complaintRepository;
        private readonly IDailyComplaintRepository _dailyComplaintRepository = dailyComplaintRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IComplaintStatusRepository _complaintStatusRepository = complaintStatusRepository;
        private readonly IDailyComplaintStatusRepository _dailyComplaintStatusRepository = dailyComplaintStatusRepository;
        private readonly IClaimsService _claimsService = claimsService;
        private readonly IMapper _mapper = mapper;
        public async Task<FeedbackDto> AddFeedbackAsync(FeedbackVm feedbackVm)
        {
            if (!feedbackVm.ComplaintId.HasValue && !feedbackVm.DailyComplaintId.HasValue)
                throw new WelfareTrackerException("Any one of the ID's should be provided.", (int)HttpStatusCode.BadRequest);
            else if (feedbackVm.ComplaintId.HasValue && feedbackVm.DailyComplaintId.HasValue)
                throw new WelfareTrackerException("only one of the ID's should be provided.", (int)HttpStatusCode.BadRequest);


            var userId = await _claimsService.GetUserIdFromClaimsAsync();

            var complaint = feedbackVm.ComplaintId.HasValue
            ? await _complaintRepository.GetComplaintByIdAsync(feedbackVm.ComplaintId.Value) : null;
            var dailyComplaint = feedbackVm.DailyComplaintId.HasValue
                    ? await _dailyComplaintRepository.GetDailyComplaintByIdAsync(feedbackVm.DailyComplaintId.Value)
                    : null;

            if (feedbackVm.ComplaintId > 0 && complaint == null)
                throw new WelfareTrackerException("complaint not found", (int)HttpStatusCode.NotFound);

            if (feedbackVm.DailyComplaintId > 0 && dailyComplaint == null)
                throw new WelfareTrackerException("daily complaint not found", (int)HttpStatusCode.NotFound);

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (complaint != null && feedbackVm.ComplaintId.HasValue && complaint.ComplaintId != user!.ConstituencyId)
                throw new WelfareTrackerException("You can only provide feedback for your constituency.", (int)HttpStatusCode.Unauthorized);

            var existingFeedback = await _feedbackRepository.GetFeedbackByUserAsync(
            userId,
            feedbackVm.ComplaintId,
            feedbackVm.DailyComplaintId
            );

            if (existingFeedback != null)
                throw new WelfareTrackerException("You have already submitted feedback for this complaint.", (int)HttpStatusCode.BadRequest);

            if (complaint != null && feedbackVm.ComplaintId > 0)
            {
                var complaintStatus = await _complaintStatusRepository.GetComplaintStatusByComplaintIdAsync(complaint.ComplaintId);
                if (complaintStatus == null || complaintStatus.Status != (int)Status.Resolved)
                    throw new WelfareTrackerException("You can provide feedback only after the complaint is resolved.", (int)HttpStatusCode.BadRequest);
            }

            if (dailyComplaint != null && feedbackVm.DailyComplaintId > 0)
            {
                var complaintStatus = await _dailyComplaintStatusRepository.GetDailyComplaintStatusByDailyComplaintIdAsync(dailyComplaint.DailyComplaintId);
                if (complaintStatus == null || complaintStatus.Status != (int)Status.Resolved)
                    throw new WelfareTrackerException("You can provide feedback only after the complaint is resolved.", (int)HttpStatusCode.BadRequest);
            }

            var complaintFeedback = new ComplaintFeedback
            {
                CitizenId = userId,
                ComplaintId = feedbackVm.ComplaintId,
                DailyComplaintId = feedbackVm.DailyComplaintId,
                FeedbackMessage = feedbackVm.FeedbackMessage,
                IsSatisfied = feedbackVm.IsSatisfied,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            };

            var addedComplaintFeedback = await _feedbackRepository.AddFeedbackAsync(complaintFeedback);

            var feedbackDto = _mapper.Map<FeedbackDto>(addedComplaintFeedback);
            return feedbackDto;
        }

        public Task EvaluateDailyComplaintFeedback()
        {
            throw new NotImplementedException();
        }

        public Task EvaluateFeedback(int complaintId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<FeedbackDto>?> GetAllFeedbacksAsync(int? complaintId, int? dailyComplaintId)
        {
            if (!complaintId.HasValue && !dailyComplaintId.HasValue)
                throw new WelfareTrackerException("Any one of the ID's should be provided.", (int)HttpStatusCode.BadRequest);
            else if (complaintId.HasValue && dailyComplaintId.HasValue)
                throw new WelfareTrackerException("only one of the ID's should be provided.", (int)HttpStatusCode.BadRequest);
            
            var userId = await _claimsService.GetUserIdFromClaimsAsync();
            var complaintFeedbacks = await _feedbackRepository.GetAllFeedbacksAsync(complaintId, dailyComplaintId);
            if (complaintFeedbacks == null)
                return null;

            return complaintFeedbacks
                    .OrderByDescending(f => f.CitizenId == userId)
                    .Select(f => _mapper.Map<FeedbackDto>(f)).ToList();
        }

        public async Task<FeedbackDto?> GetFeedbackByUserAsync(int? complaintId, int? dailyComplaintId)
        {
            if (!complaintId.HasValue && !dailyComplaintId.HasValue)
                throw new WelfareTrackerException("Any one of the ID's should be provided.", (int)HttpStatusCode.BadRequest);
            else if (complaintId.HasValue && dailyComplaintId.HasValue)
                throw new WelfareTrackerException("only one of the ID's should be provided.", (int)HttpStatusCode.BadRequest);

            var userId = await _claimsService.GetUserIdFromClaimsAsync();
            var complaintFeedback = await _feedbackRepository.GetFeedbackByUserAsync(userId, complaintId, dailyComplaintId);
            if (complaintFeedback == null)
                return null;

            var feedbackDto = _mapper.Map<FeedbackDto>(complaintFeedback);
            return feedbackDto;
        }
    }
}
