using AutoMapper;
using System.Net;
using WelfareWorkTracker.Core.Contracts.Repository;
using WelfareWorkTracker.Core.Contracts.Service;
using WelfareWorkTracker.Core.Dtos;
using WelfareWorkTracker.Core.Enums;
using WelfareWorkTracker.Core.Exceptions;
using WelfareWorkTracker.Core.Models;
using WelfareWorkTracker.Core.Vms;

namespace WelfareWorkTracker.Infrastructure.Service
{
    public class FeedbackService(IFeedbackRepository FeedbackRepository,
                                    IComplaintRepository complaintRepository,
                                    IDailyComplaintRepository dailyComplaintRepository,
                                    IComplaintStatusRepository complaintStatusRepository,
                                    IDailyComplaintStatusRepository dailyComplaintStatusRepository,
                                    IClaimsService claimsService,
                                    IUserRepository userRepository,
                                    IConstituencyRepository constituencyRepository,
                                    IMapper mapper) : IFeedbackService
    {
        private readonly IFeedbackRepository _FeedbackRepository = FeedbackRepository;
        private readonly IComplaintRepository _complaintRepository = complaintRepository;
        private readonly IDailyComplaintRepository _dailyComplaintRepository = dailyComplaintRepository;
        private readonly IComplaintStatusRepository _complaintStatusRepository = complaintStatusRepository;
        private readonly IDailyComplaintStatusRepository _dailyComplaintStatusRepository = dailyComplaintStatusRepository;
        private readonly IClaimsService _claimsService = claimsService;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IConstituencyRepository _constituencyRepository = constituencyRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<ComplaintFeedbackDto> AddFeedbackAsync(ComplaintFeedbackVm citizenFeedbackVm)
        {
            if (!citizenFeedbackVm.ComplaintId.HasValue && !citizenFeedbackVm.DailyComplaintId.HasValue)
                throw new WelfareWorkTrackerException("Any one of the ID's should be provided.", (int)HttpStatusCode.BadRequest);
            else if (citizenFeedbackVm.ComplaintId.HasValue && citizenFeedbackVm.DailyComplaintId.HasValue)
                throw new WelfareWorkTrackerException("only one of the ID's should be provided.", (int)HttpStatusCode.BadRequest);
            
            var userId = await _claimsService.GetUserIdFromClaimsAsync();

            var complaint = citizenFeedbackVm.ComplaintId.HasValue
                    ? await _complaintRepository.GetComplaintByComplaintIdAsync(citizenFeedbackVm.ComplaintId.Value) : null;
            var dailyComplaint = citizenFeedbackVm.DailyComplaintId.HasValue
                    ? await _dailyComplaintRepository.GetDailyComplaintByIdAsync(citizenFeedbackVm.DailyComplaintId.Value) : null;

            if (citizenFeedbackVm.ComplaintId > 0 && complaint == null)
                throw new WelfareWorkTrackerException("complaint not found", (int)HttpStatusCode.NotFound);

            if (citizenFeedbackVm.DailyComplaintId > 0 && dailyComplaint == null)
                throw new WelfareWorkTrackerException("daily complaint not found", (int)HttpStatusCode.NotFound);

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (complaint != null && citizenFeedbackVm.ComplaintId.HasValue && complaint.ConstituencyId != user!.ConstituencyId)
                throw new WelfareWorkTrackerException("You can only provide feedback for your constituency.", (int)HttpStatusCode.Unauthorized);


            var existingFeedback = await _FeedbackRepository.GetFeedbackByUserAsync(
                userId,
                citizenFeedbackVm.ComplaintId,
                citizenFeedbackVm.DailyComplaintId
            );

            if (existingFeedback != null)
                throw new WelfareWorkTrackerException("You have already submitted feedback for this complaint.", (int)HttpStatusCode.BadRequest);

            if (complaint != null && citizenFeedbackVm.ComplaintId > 0)
            {
                var complaintStatus = await _complaintStatusRepository.GetComplaintStatusByComplaintIdAsync(complaint.ComplaintId);
                if (complaintStatus == null || complaintStatus.Status != (int)Status.Resolved)
                    throw new WelfareWorkTrackerException("You can provide feedback only after the complaint is resolved.", (int)HttpStatusCode.BadRequest);
            }

            if (dailyComplaint != null && citizenFeedbackVm.DailyComplaintId > 0)
            {
                var complaintStatus = await _dailyComplaintStatusRepository.GetDailyComplaintStatusByDailyComplaintStatusIdAsync(dailyComplaint.DailyComplaintId);
                if (complaintStatus == null || complaintStatus.Status != (int)Status.Resolved)
                    throw new WelfareWorkTrackerException("You can provide feedback only after the complaint is resolved.", (int)HttpStatusCode.BadRequest);
            }


            var complaintFeedback = new ComplaintFeedback
            {
                CitizenId = userId,
                ComplaintId = citizenFeedbackVm.ComplaintId,
                DailyComplaintId = citizenFeedbackVm.DailyComplaintId,
                FeedbackMessage = citizenFeedbackVm.FeedbackMessage,
                IsSatisfied = citizenFeedbackVm.IsSatisfied,
                DateCreated = DateTime.UtcNow,
            };

            var addedComplaintFeedback = await _FeedbackRepository.AddFeedbackAsync(complaintFeedback);

            var complaintFeedbackDto = _mapper.Map<ComplaintFeedbackDto>(addedComplaintFeedback);
            return complaintFeedbackDto;

        }
        public async Task<List<ComplaintFeedbackDto>?> GetAllFeedbacksAsync(int? complaintId, int? dailyComplaintId)
        {
            if (!complaintId.HasValue && !dailyComplaintId.HasValue)
                throw new WelfareWorkTrackerException("Any one of the ID's should be provided.", (int)HttpStatusCode.BadRequest);
            else if (complaintId.HasValue && dailyComplaintId.HasValue)
                throw new WelfareWorkTrackerException("only one of the ID's should be provided.", (int)HttpStatusCode.BadRequest);
            var userId = await _claimsService.GetUserIdFromClaimsAsync();
            var complaintFeedbacks = await _FeedbackRepository.GetAllFeedbacksAsync(complaintId, dailyComplaintId);
            if (complaintFeedbacks == null)
                return null;

            return complaintFeedbacks
                        .OrderByDescending(f => f.CitizenId == userId)
                        .Select(f => _mapper.Map<ComplaintFeedbackDto>(f)).ToList();
        }

        public async Task<ComplaintFeedbackDto?> GetFeedbackByUserAsync(int? complaintId, int? dailyComplaintId)
        {
            if (!complaintId.HasValue && !dailyComplaintId.HasValue)
                throw new WelfareWorkTrackerException("Any one of the ID's should be provided.", (int)HttpStatusCode.BadRequest);
            else if (complaintId.HasValue && dailyComplaintId.HasValue)
                throw new WelfareWorkTrackerException("only one of the ID's should be provided.", (int)HttpStatusCode.BadRequest);
            var userId = await _claimsService.GetUserIdFromClaimsAsync();
            var complaintFeedback = await _FeedbackRepository.GetFeedbackByUserAsync(userId, complaintId, dailyComplaintId);
            if (complaintFeedback == null)
                return new ComplaintFeedbackDto();

            var complaintFeedbackDto = _mapper.Map<ComplaintFeedbackDto>(complaintFeedback);
            return complaintFeedbackDto;
        }

        public async Task EvaluateFeedback(int complaintId)
        {
            var complaint = await _complaintRepository.GetComplaintByComplaintIdAsync(complaintId) 
                                    ?? throw new WelfareWorkTrackerException($"No complaint found with complaintId: {complaintId}");
            var complaintStatus = await _complaintStatusRepository.GetComplaintStatusByComplaintIdAsync(complaintId);

            var constituencyName = await _constituencyRepository.GetConstituencyNameByIdAsync(complaint.ComplaintId);
            var leader = await _userRepository.GetLeaderByConstituencyNameAsync(constituencyName!);

            if (leader!.Reputation <= 15)
            {
                return;
            }

            // Checking if the complaint is resolved
            if (complaintStatus!.Status == (int)Status.Resolved)
            {
                int population = await _userRepository.GetCitizenCountInConstituencyAsync(complaint.ConstituencyId);

                float satisfiedThreshold = population * 0.25f;
                float unsatisfiedThreshold = satisfiedThreshold * 0.10f;
                float satisfiedCount = await _FeedbackRepository.GetSatisfiedCount(complaintId);
                float unsatisfiedCount = await _FeedbackRepository.GetUnSatisfiedCount(complaintId);

                // Checking if 10 days have passed since the complaint was resolved
                if (DateTime.UtcNow >= complaintStatus.DateCreated)
                {
                    float satisfiedFeedback = (satisfiedCount / satisfiedThreshold) * (satisfiedCount * 0.01f * population);
                    float unsatisfiedFeedback = (unsatisfiedCount / unsatisfiedThreshold) * (unsatisfiedCount * 0.01f * population);

                    if (satisfiedFeedback > unsatisfiedFeedback)
                    {
                        await AddComplaintStatusRecordAsync(complaintId, Status.Closed);
                        int value = (int)Math.Ceiling(satisfiedCount * 0.01f * population);

                        if (leader.Reputation < 100)
                        {
                            leader.Reputation += value * 0.01;

                            if (leader.Reputation > 100)
                                leader.Reputation = 100;

                            await _userRepository.UpdateLeaderReputationAsync(leader.UserId, leader.Reputation);
                        }

                    }
                    else if (unsatisfiedFeedback > satisfiedFeedback)
                    {
                        complaint.Attempts++;

                        if (complaint.Attempts >= 4)
                        {
                            await AddComplaintStatusRecordAsync(complaintId, Status.Unresolved);

                            var unResolvedComplaints = await _complaintRepository.GetUnresolvedComplaintsForLeaderAsync(leader.UserId);

                            int unResolvedCount = unResolvedComplaints.Count;

                            if (unResolvedCount > 3)
                            {
                                leader.Reputation -= 50;
                                if (leader.Reputation < 0)
                                {
                                    leader.Reputation = 0;
                                }
                            }
                            else
                            {
                                leader.Reputation -= 40;
                            }
                        }
                        else
                        {
                            // Reopening the complaint
                            complaint.Attempts++;
                            await AddComplaintStatusRecordAsync(complaintStatus.ComplaintId, Status.Reopened);

                        }
                        await _complaintRepository.UpdateComplaintAsync(complaint);
                        var attemptNumber = complaint.Attempts;

                        // Decrease reputation based on the attempt number
                        if (attemptNumber == 2 || attemptNumber == 3 || attemptNumber == 4)
                        {
                            await HandleReputationReductionAsync(attemptNumber, leader, complaint, unsatisfiedCount, population);
                        }
                        await _userRepository.UpdateLeaderReputationAsync(leader.UserId, leader.Reputation);

                        if (leader.Reputation < 15)
                        {
                            //await TriggerReElectionAsync(leader)
                        }
                    }
                }
                else
                {
                    throw new WelfareWorkTrackerException();
                }
            }
        }

        private static async Task HandleReputationReductionAsync(int attemptNumber, User leader, Complaint complaint, float unsatisfiedCount, int population)
        {

            int value = (int)Math.Ceiling(unsatisfiedCount * 0.01f * population);
            double reductionMultiplier = Math.Pow(2, attemptNumber - 2);

            // Reduce the reputation
            leader.Reputation -= reductionMultiplier * value * 0.01;
        }

        public async Task EvaluateDailyComplaintFeedback()
        {
            var todaysDailyComplaint = await _dailyComplaintRepository.GetTodaysDailyComplaintsAsync();
            foreach (var dailyComplaint in todaysDailyComplaint)
            {
                var dailyComplaintStatus = await _dailyComplaintStatusRepository.GetDailyComplaintStatusByDailyComplaintIdAsync(dailyComplaint.DailyComplaintId) 
                                                    ?? throw new WelfareWorkTrackerException("Daily Complaint Status Not Found.");
                var constituencyName = await _constituencyRepository.GetConstituencyNameByIdAsync(dailyComplaint.ConstituencyId);

                var leader = await _userRepository.GetLeaderByConstituencyNameAsync(constituencyName!);

                if (leader!.Reputation <= 15)
                {
                    continue;
                }

                // Checking if the complaint is resolved
                if (dailyComplaintStatus.Status == (int)Status.Resolved)
                {
                    int population = await _userRepository.GetCitizenCountInConstituencyAsync(dailyComplaint.ConstituencyId);

                    float satisfiedThreshold = population * 0.25f;
                    float unsatisfiedThreshold = satisfiedThreshold * 0.10f;

                    float satisfiedCount = await _FeedbackRepository.GetSatisfiedCount(null, dailyComplaint.DailyComplaintId);
                    float unsatisfiedCount = await _FeedbackRepository.GetUnSatisfiedCount(null, dailyComplaint.DailyComplaintId);

                    float satisfiedFeedback = (satisfiedCount / satisfiedThreshold) * (satisfiedCount * 0.01f * population);
                    float unsatisfiedFeedback = (unsatisfiedCount / unsatisfiedThreshold) * (unsatisfiedCount * 0.01f * population);

                    if (satisfiedFeedback > unsatisfiedFeedback)
                    {
                        int value = (int)Math.Ceiling(satisfiedCount * 0.01f * population);

                        leader.Reputation += value * 0.01;

                        if (leader.Reputation > 100)
                            leader.Reputation = 100;
                    }
                    else if (unsatisfiedFeedback > satisfiedFeedback)
                    {
                        var unResolvedComplaints = await _dailyComplaintStatusRepository.GetUnresolvedComplaintsOfLeader(leader.UserId);

                        int unResolvedCount = unResolvedComplaints.Count;

                        if (unResolvedCount > 3)
                        {
                            leader.Reputation -= 5;
                        }
                        else
                        {
                            int value = (int)Math.Ceiling(unsatisfiedCount * 0.01f * population);

                            leader.Reputation -= Math.Min(value * 0.01, 5);
                        }
                    }

                    await _userRepository.UpdateLeaderReputationAsync(leader.UserId, leader.Reputation);

                    if (leader.Reputation < 15)
                    {
                        //await TriggerReElectionAsync(leader)
                    }
                }
            }
        }

        private async Task AddComplaintStatusRecordAsync(int complaintId, Status newStatus)
        {
            var complaint = await _complaintRepository.GetComplaintByComplaintIdAsync(complaintId) 
                                    ?? throw new WelfareWorkTrackerException($"No complaint found with complaint Id: {complaintId}");

            var complaintStatus = new ComplaintStatus
            {
                ComplaintId = complaintId,
                Status = (int)newStatus,
                AttemptNumber = complaint.Attempts,
                DateCreated = DateTime.UtcNow
            };

            complaint.Status = (int)newStatus;
            complaint.OpenedDate = DateTime.UtcNow;
            await _complaintRepository.UpdateComplaintAsync(complaint);

            await _complaintStatusRepository.AddComplaintStatusAsync(complaintStatus);
        }
    }
}
