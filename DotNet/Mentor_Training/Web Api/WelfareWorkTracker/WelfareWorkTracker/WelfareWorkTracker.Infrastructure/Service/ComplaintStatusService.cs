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
    public class ComplaintStatusService(IComplaintStatusRepository complaintStatusRepository,
                                            IComplaintRepository complaintRepository,
                                            IUserRepository userRepository,
                                            IComplaintService complaintService,
                                            IClaimsService claimsService,
                                            IMapper mapper) : IComplaintStatusService
    {
        private readonly IComplaintStatusRepository _complaintStatusRepository = complaintStatusRepository;
        private readonly IComplaintRepository _complaintRepository = complaintRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IComplaintService _complaintService = complaintService;
        private readonly IClaimsService _claimsService = claimsService;
        private readonly IMapper _mapper = mapper;
        public async Task<ComplaintStatusDto> AddComplaintStatusByAdmin(ComplaintStatusAdminVm complaintStatusAdminVm)
        {
            if (complaintStatusAdminVm.Status != (int)Status.Backlog && complaintStatusAdminVm.Status != (int)Status.Reject)
            {
                throw new WelfareWorkTrackerException("Status must be either Backlog or Rejected for complaint.", (int)HttpStatusCode.Unauthorized);
            }

            var complaint = await _complaintRepository.GetComplaintByComplaintIdAsync(complaintStatusAdminVm.ComplaintId) 
                                        ?? throw new WelfareWorkTrackerException("Complaint not found.", (int)HttpStatusCode.NotFound);

            if (complaint.Status != (int)Status.Valid && complaint.Status != (int)Status.Invalid && complaint.Status != (int)Status.Reopened)
            {
                throw new WelfareWorkTrackerException("Cannot update status of the complaint", (int)HttpStatusCode.Unauthorized);
            }

            var existingComplaintStatus = await _complaintStatusRepository.GetComplaintStatusByComplaintIdAsync(complaintStatusAdminVm.ComplaintId) 
                                                    ?? throw new WelfareWorkTrackerException("No status found for the provided complaint Id", (int)HttpStatusCode.NotFound);

            if (complaint.Status == (int)Status.Invalid)
            {
                var newStatus = new ComplaintStatus
                {
                    ComplaintId = complaintStatusAdminVm.ComplaintId,
                    Status = (int)Status.Reject,
                    AttemptNumber = complaint.Attempts,
                    RejectReason = complaintStatusAdminVm.RejectReason,
                    DeadlineDate = existingComplaintStatus.DeadlineDate,
                    DateCreated = DateTime.UtcNow
                };
                await _complaintStatusRepository.AddComplaintStatusAsync(newStatus);
                complaint.Status = (int)Status.Reject;
                await _complaintRepository.UpdateComplaintAsync(complaint);
                await _complaintService.UpdateStatusOfAllReferredComplaintsAsync(complaintStatusAdminVm.ComplaintId);
                var complaintStatusDto = _mapper.Map<ComplaintStatusDto>(newStatus);
                return complaintStatusDto;

            }
            else
            {
                var newStatus = new ComplaintStatus
                {
                    ComplaintId = complaintStatusAdminVm.ComplaintId,
                    Status = complaintStatusAdminVm.Status,
                    AttemptNumber = complaint.Attempts,
                    DeadlineDate = existingComplaintStatus.DeadlineDate,
                    DateCreated = DateTime.UtcNow
                };
                if (complaintStatusAdminVm.Status == (int)Status.Reject)
                {
                    newStatus.RejectReason = complaintStatusAdminVm.RejectReason;
                }
                else
                {
                    complaint.OpenedDate = DateTime.UtcNow;
                }
                await _complaintStatusRepository.AddComplaintStatusAsync(newStatus);
                complaint.Status = complaintStatusAdminVm.Status;
                await _complaintRepository.UpdateComplaintAsync(complaint);
                await _complaintService.UpdateStatusOfAllReferredComplaintsAsync(complaintStatusAdminVm.ComplaintId);

                var complaintStatusDto = _mapper.Map<ComplaintStatusDto>(newStatus);
                return complaintStatusDto;
            }
        }

        public async Task<ComplaintStatusDto> AddComplaintStatusByAdminRep(ComplaintStatusAdminRepVm complaintStatusAdminRepVm)
        {
            if (complaintStatusAdminRepVm.Status != (int)Status.Valid && complaintStatusAdminRepVm.Status != (int)Status.Invalid)
            {
                throw new WelfareWorkTrackerException("Status must be either Valid or Invalid for referenced complaints.");
            }

            var complaint = await _complaintRepository.GetComplaintByComplaintIdAsync(complaintStatusAdminRepVm.ComplaintId) 
                ?? throw new WelfareWorkTrackerException("Complaint not found.", (int)HttpStatusCode.NotFound);
            complaint.ReferenceNumber = complaintStatusAdminRepVm.ReferenceNumber;
            await _complaintRepository.UpdateComplaintAsync(complaint);

            var userId = await _claimsService.GetUserIdFromClaimsAsync();
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (complaint.ConstituencyId != user!.ConstituencyId)
            {
                throw new WelfareWorkTrackerException("You can only update complaints in your constituency.", (int)HttpStatusCode.Unauthorized);
            }

            if (complaint.ReferenceNumber != 0)
            {
                var linkedcomplaintStatus = await _complaintStatusRepository.GetComplaintStatusByComplaintIdAsync(complaintStatusAdminRepVm.ReferenceNumber) 
                                                        ?? throw new WelfareWorkTrackerException("Referenced complaint status not found.", (int)HttpStatusCode.NotFound);

                if (linkedcomplaintStatus.Status == (int)Status.Unresolved || linkedcomplaintStatus.Status == (int)Status.UnderValidation)
                {
                    var newComplaintStatus = new ComplaintStatus
                    {
                        ComplaintId = linkedcomplaintStatus.ComplaintId,
                        Status = linkedcomplaintStatus.Status,
                        AttemptNumber = 1,
                        DeadlineDate = DateTime.UtcNow.AddDays((double)complaintStatusAdminRepVm.ExpectedDeadline!),
                        DateCreated = DateTime.UtcNow
                    };

                    await _complaintStatusRepository.AddComplaintStatusAsync(newComplaintStatus);

                    var linkedComplaint = await _complaintRepository.GetComplaintByComplaintIdAsync(complaintStatusAdminRepVm.ReferenceNumber) 
                        ?? throw new WelfareWorkTrackerException("Referenced complaint not found.", (int)HttpStatusCode.NotFound);

                    linkedComplaint.Status = complaintStatusAdminRepVm.Status;
                    await _complaintRepository.UpdateComplaintAsync(linkedComplaint);

                    var referredComplaintStatusUpdate = await _complaintService.UpdateStatusOfAllReferredComplaintsAsync(complaintStatusAdminRepVm.ReferenceNumber);
                    if (!referredComplaintStatusUpdate)
                    {
                        throw new WelfareWorkTrackerException("Failed to update referred complaints statuses.");
                    }

                    var complaintStatusDto = _mapper.Map<ComplaintStatusDto>(linkedcomplaintStatus);
                    return complaintStatusDto;
                }
                else
                {
                    throw new WelfareWorkTrackerException("Cannot update status of complaints with status other than UnderValidation or Unresolved.", (int)HttpStatusCode.Unauthorized);
                }
            }
            else
            {
                var complaintToUpdate = await _complaintRepository.GetComplaintByComplaintIdAsync(complaintStatusAdminRepVm.ComplaintId) 
                                                        ?? throw new WelfareWorkTrackerException("Complaint not found.", (int)HttpStatusCode.NotFound);

                if (complaintToUpdate.Status != (int)Status.UnderValidation && complaintToUpdate.Status != (int)Status.Unresolved)
                {
                    throw new WelfareWorkTrackerException("Complaint Status cannot be updated.", (int)HttpStatusCode.Unauthorized);
                }
                complaintToUpdate.Status = complaintStatusAdminRepVm.Status;
                await _complaintRepository.UpdateComplaintAsync(complaintToUpdate);
                var newComplaintStatus = new ComplaintStatus
                {
                    ComplaintId = complaintStatusAdminRepVm.ComplaintId,
                    Status = complaintStatusAdminRepVm.Status,
                    AttemptNumber = complaintToUpdate.Attempts,
                    DeadlineDate = DateTime.UtcNow.AddDays((double)complaintStatusAdminRepVm.ExpectedDeadline!),
                    DateCreated = DateTime.UtcNow
                };

                await _complaintStatusRepository.AddComplaintStatusAsync(newComplaintStatus);

                var complaintStatusDto = _mapper.Map<ComplaintStatusDto>(newComplaintStatus);
                return complaintStatusDto;
            }
        }

        public async Task<ComplaintStatusDto> AddComplaintStatusByLeader(ComplaintStatusLeaderVm complaintStatusLeaderVm)
        {
            if (complaintStatusLeaderVm.Status != (int)Status.Resolved && complaintStatusLeaderVm.Status != (int)Status.InProgress)
            {
                throw new WelfareWorkTrackerException("Status must be either Backlog or InProgress for complaint.", (int)HttpStatusCode.Unauthorized);
            }

            var userId = await _claimsService.GetUserIdFromClaimsAsync();
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null || user.RoleName != "Leader")
            {
                throw new WelfareWorkTrackerException("Only Leaders can update complaint status.", (int)HttpStatusCode.Unauthorized);
            }

            var complaint = await _complaintRepository.GetComplaintByComplaintIdAsync(complaintStatusLeaderVm.ComplaintId) 
                                        ?? throw new WelfareWorkTrackerException("Complaint not found.", (int)HttpStatusCode.NotFound);

            var complaintStatus = await _complaintStatusRepository.GetComplaintStatusByComplaintIdAsync(complaintStatusLeaderVm.ComplaintId) 
                                                ?? throw new WelfareWorkTrackerException("Complaint status not found.", (int)HttpStatusCode.NotFound);

            if (user.ConstituencyId != complaint.ConstituencyId)
            {
                throw new WelfareWorkTrackerException("You can only update complaints in your constituency.", (int)HttpStatusCode.Unauthorized);
            }

            if (complaint.Status != (int)Status.Backlog && complaint.Status != (int)Status.InProgress)
            {
                throw new WelfareWorkTrackerException("Cannot update status of the complaint");
            }

            if (complaint.Status == (int)Status.Backlog)
            {
                var validDate = DateTime.UtcNow - complaint.OpenedDate;
                if (validDate!.Value.TotalDays > 6)
                {
                    throw new WelfareWorkTrackerException("Cannot update complaint status after 7 days of opening.", (int)HttpStatusCode.Unauthorized);
                }
                if (complaintStatusLeaderVm.Status == (int)Status.InProgress)
                {
                    complaint.Status = complaintStatusLeaderVm.Status;
                    await _complaintRepository.UpdateComplaintAsync(complaint);
                    var newStatus = new ComplaintStatus
                    {
                        ComplaintId = complaintStatusLeaderVm.ComplaintId,
                        Status = complaintStatusLeaderVm.Status,
                        AttemptNumber = complaint.Attempts,
                        DeadlineDate = complaintStatus.DeadlineDate,
                        DateCreated = DateTime.UtcNow
                    };
                    await _complaintStatusRepository.AddComplaintStatusAsync(newStatus);
                    complaint.Status = complaintStatusLeaderVm.Status;
                    await _complaintRepository.UpdateComplaintAsync(complaint);
                    await _complaintService.UpdateStatusOfAllReferredComplaintsAsync(complaintStatusLeaderVm.ComplaintId);

                    var complaintStatusDto = _mapper.Map<ComplaintStatusDto>(newStatus);
                    return complaintStatusDto;
                }
                else
                {
                    throw new WelfareWorkTrackerException("Invalid status update from Backlog.");
                }
            }

            else
            {
                if (complaintStatusLeaderVm.Status == (int)Status.Resolved)
                {
                    complaint.Status = complaintStatusLeaderVm.Status;
                    await _complaintRepository.UpdateComplaintAsync(complaint);
                    var newStatus = new ComplaintStatus
                    {
                        ComplaintId = complaintStatusLeaderVm.ComplaintId,
                        Status = complaintStatusLeaderVm.Status,
                        AttemptNumber = complaint.Attempts,
                        DeadlineDate = complaintStatus.DeadlineDate,
                        DateCreated = DateTime.UtcNow
                    };
                    await _complaintStatusRepository.AddComplaintStatusAsync(newStatus);
                    complaint.Status = complaintStatusLeaderVm.Status;
                    await _complaintRepository.UpdateComplaintAsync(complaint);
                    await _complaintService.UpdateStatusOfAllReferredComplaintsAsync(complaintStatusLeaderVm.ComplaintId);

                    var complaintStatusDto = _mapper.Map<ComplaintStatusDto>(newStatus);
                    return complaintStatusDto;
                }
                else
                {
                    throw new WelfareWorkTrackerException("Invalid status update from InProgress.");
                }
            }
        }

        public async Task<ComplaintStatusDto> GetComplaintStatusByComplaintId(int complaintId)
        {
            var complaintStatus = await _complaintStatusRepository.GetComplaintStatusByComplaintIdAsync(complaintId)
                ?? throw new WelfareWorkTrackerException("Complaint not found.", (int)HttpStatusCode.NotFound);

            var complaintStatusDto = _mapper.Map<ComplaintStatusDto>(complaintStatus);
            return complaintStatusDto;
        }

        public async Task<ComplaintStatusDto> GetComplaintStatusByIdAsync(int complaintStatusId)
        {
            var complaintStatus = await _complaintStatusRepository.GetComplaintStatusByIdAsync(complaintStatusId)
                ?? throw new WelfareWorkTrackerException("Complaint not found.", (int)HttpStatusCode.NotFound);

            var complaintStatusDto = _mapper.Map<ComplaintStatusDto>(complaintStatus);
            return complaintStatusDto;
        }

        public async Task<List<ComplaintStatusDto>> GetComplaintStatusHistoryAsync(int complaintId)
        {
            var complaintStatuses = await _complaintStatusRepository.GetComplaintStatusHistoryByComplaintIdAsync(complaintId);
            if (complaintStatuses == null || complaintStatuses.Count == 0)
                throw new WelfareWorkTrackerException("Complaint Status not found.", (int)HttpStatusCode.NotFound);

            var complaintStatusDtos = new List<ComplaintStatusDto>();

            foreach(var complaintStatus in complaintStatuses)
            {
                var complaintStatusDto = _mapper.Map<ComplaintStatusDto>(complaintStatus);
                complaintStatusDtos.Add(complaintStatusDto);
            }
            return complaintStatusDtos;
        }
    }
}
