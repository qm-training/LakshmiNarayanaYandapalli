using WelfareTracker.Core.Contracts.Repository;
using WelfareTracker.Core.Contracts.Service;
using WelfareTracker.Core.Dtos;
using WelfareTracker.Core.Enums;
using WelfareTracker.Core.Exceptions;
using WelfareTracker.Core.Models;
using WelfareTracker.Core.Vms;

namespace WelfareTracker.Infrastructure.Service
{
    public class ComplaintStatusService(IComplaintStatusRepository complaintStatusRepository,
                                            IComplaintRepository complaintRepository,
                                            IComplaintService complaintService,
                                            IClaimsService claimsService,
                                            IUserRepository userRepository) : IComplaintStatusService
    {
        private readonly IComplaintStatusRepository _complaintStatusRepository = complaintStatusRepository;
        private readonly IComplaintRepository _complaintRepository = complaintRepository;
        private readonly IComplaintService _complaintService = complaintService;
        private readonly IClaimsService _claimsService = claimsService;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<ComplaintDto?> AddComplaintStatusByAdmin(AdminStatusVm adminStatusVm)
        {
            if (adminStatusVm.Status != (int)Status.Backlog && adminStatusVm.Status != (int)Status.Reject)
            {
                throw new WelfareTrackerException("Status must be either Backlog or Rejected for referenced complaints.");
            }

            var userId = await _claimsService.GetUserIdFromClaimsAsync();
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null || user.RoleName != "Admin")
            {
                throw new WelfareTrackerException("Only Admin can update complaint status.");
            }

            var complaint = await _complaintRepository.GetComplaintByIdAsync(adminStatusVm.ComplaintId) ?? throw new WelfareTrackerException("Complaint not found.");

            if(complaint.Status != (int)Status.Valid && complaint.Status != (int)Status.Invalid && complaint.Status != (int)Status.Reopened)
            {
                throw new WelfareTrackerException("Cannot update status of the complaint");
            }

            var existingComplaintStatus = await _complaintStatusRepository.GetComplaintStatusByComplaintIdAsync(adminStatusVm.ComplaintId) ?? throw new WelfareTrackerException("No status found for thr provided complaint Id");

            if (complaint.Status == (int)Status.Invalid)
            {
                var newStatus = new ComplaintStatus
                {
                    ComplaintId = adminStatusVm.ComplaintId,
                    Status = (int)Status.Reject,
                    AttemptNumber = complaint.Attempts,
                    RejectReason = adminStatusVm.RejectReason,
                    DeadlineDate = existingComplaintStatus.DeadlineDate,
                    DateCreated = DateTime.UtcNow
                };
                await _complaintStatusRepository.AddComplaintStatusAsync(newStatus);
                complaint.Status = (int)Status.Reject;
                await _complaintRepository.UpdateComplaintAsync(complaint);
                await _complaintService.UpdateStatusOfReferredComplaintsAsync(adminStatusVm.ComplaintId);
            }
            else
            {
                var newStatus = new ComplaintStatus
                {
                    ComplaintId = adminStatusVm.ComplaintId,
                    Status = adminStatusVm.Status,
                    AttemptNumber = complaint.Attempts,
                    DeadlineDate = existingComplaintStatus.DeadlineDate,
                    DateCreated = DateTime.UtcNow
                };
                if(adminStatusVm.Status == (int)Status.Reject)
                {
                    newStatus.RejectReason = adminStatusVm.RejectReason;
                }
                else
                {
                    complaint.OpenedDate = DateTime.UtcNow;
                }
                await _complaintStatusRepository.AddComplaintStatusAsync(newStatus);
                complaint.Status = adminStatusVm.Status;
                await _complaintRepository.UpdateComplaintAsync(complaint);
                await _complaintService.UpdateStatusOfReferredComplaintsAsync(adminStatusVm.ComplaintId);
            }
            var updatedComplaint = await _complaintService.GetComplaintByComplaintIdAsync(adminStatusVm.ComplaintId);
            return updatedComplaint;
        }

        public async Task<ComplaintDto?> AddComplaintStatusByAdminRep(AdminRepStatusVm adminRepStatusVm)
        {
            if (adminRepStatusVm.Status != (int)Status.Valid && adminRepStatusVm.Status != (int)Status.Invalid)
            {
                throw new WelfareTrackerException("Status must be either Valid or Invalid for referenced complaints.");
            }

            var userId = await _claimsService.GetUserIdFromClaimsAsync();
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null || user.RoleName != "AdminRepresentative")
            {
                throw new WelfareTrackerException("Only Admin Representatives can update complaint status.");
            }

            var complaint = await _complaintRepository.GetComplaintByIdAsync(adminRepStatusVm.ComplaintId) ?? throw new WelfareTrackerException("Complaint not found.");
            complaint.ReferenceNumber = adminRepStatusVm.ReferenceNumber;
            await _complaintRepository.UpdateComplaintAsync(complaint);

            if (complaint.ConstituencyId != user.ConstituencyId)
            {
                throw new WelfareTrackerException("You can only update complaints in your constituency.");
            }

            if (complaint.ReferenceNumber != 0) 
            { 
                var linkedcomplaintStatus = await _complaintStatusRepository.GetComplaintStatusByComplaintIdAsync(adminRepStatusVm.ReferenceNumber) ?? throw new WelfareTrackerException("Referenced complaint status not found.");                

                if (linkedcomplaintStatus.Status == (int) Status.Unresolved || linkedcomplaintStatus.Status == (int)Status.UnderValidation)
                {
                    var newComplaintStatus = new ComplaintStatus
                    {
                        ComplaintId = linkedcomplaintStatus.ComplaintId,
                        Status = linkedcomplaintStatus.Status,
                        AttemptNumber = 1,
                        DeadlineDate = DateTime.UtcNow.AddDays((double)adminRepStatusVm.ExpectedDeadline!),
                        DateCreated = DateTime.UtcNow
                    };

                    await _complaintStatusRepository.AddComplaintStatusAsync(newComplaintStatus);

                    var linkedComplaint = await _complaintRepository.GetComplaintByIdAsync(adminRepStatusVm.ReferenceNumber) ?? throw new WelfareTrackerException("Referenced complaint not found.");

                    linkedComplaint.Status = adminRepStatusVm.Status;
                    await _complaintRepository.UpdateComplaintAsync(linkedComplaint);

                    var referredComplaintStatusUpdate = await _complaintService.UpdateStatusOfReferredComplaintsAsync(adminRepStatusVm.ReferenceNumber);
                    if (!referredComplaintStatusUpdate)
                    {
                        throw new WelfareTrackerException("Failed to update referred complaints statuses.");
                    }
                }
                else
                {
                    throw new WelfareTrackerException("Cannot update status of complaints with status other than UnderValidation or Unresolved.");
                }
            }
            else
            {
                var complaintToUpdate = await _complaintRepository.GetComplaintByIdAsync(adminRepStatusVm.ComplaintId) ?? throw new WelfareTrackerException("Complaint not found.");
                if(complaintToUpdate.Status != (int)Status.UnderValidation && complaintToUpdate.Status != (int)Status.Unresolved)
                {
                    throw new WelfareTrackerException("Complaint Status cannot be updated.");
                }
                complaintToUpdate.Status = adminRepStatusVm.Status;
                await _complaintRepository.UpdateComplaintAsync(complaintToUpdate);
                var newComplaintStatus = new ComplaintStatus
                {
                    ComplaintId = adminRepStatusVm.ComplaintId,
                    Status = adminRepStatusVm.Status,
                    AttemptNumber = complaintToUpdate.Attempts,
                    DeadlineDate = DateTime.UtcNow.AddDays((double)adminRepStatusVm.ExpectedDeadline!),
                    DateCreated = DateTime.UtcNow
                };

                await _complaintStatusRepository.AddComplaintStatusAsync(newComplaintStatus);
            }

            var updatedComplaint = await _complaintService.GetComplaintByComplaintIdAsync(adminRepStatusVm.ComplaintId);

            return updatedComplaint;
        }

        public async Task<ComplaintDto?> AddComplaintStatusByLeader(LeaderStatusVm leaderStatusVm)
        {
            if(leaderStatusVm.Status != (int)Status.Resolved && leaderStatusVm.Status != (int)Status.InProgress)
            {
                throw new WelfareTrackerException("Status must be either Backlog or InProgress for complaint.");
            }

            var userId = await _claimsService.GetUserIdFromClaimsAsync();
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null || user.RoleName != "Leader")
            {
                throw new WelfareTrackerException("Only Leaders can update complaint status.");
            }

            var complaint = await _complaintRepository.GetComplaintByIdAsync(leaderStatusVm.ComplaintId) ?? throw new WelfareTrackerException("Complaint not found.");
            var complaintStatus = await _complaintStatusRepository.GetComplaintStatusByComplaintIdAsync(leaderStatusVm.ComplaintId) ?? throw new WelfareTrackerException("Complaint status not found.");

            if (user.ConstituencyId != complaint.ConstituencyId)
            {
                throw new WelfareTrackerException("You can only update complaints in your constituency.");
            }

            if(complaint.Status != (int)Status.Backlog && complaint.Status != (int)Status.InProgress)
            {
                throw new WelfareTrackerException("Cannot update status of the complaint");
            }

            if(complaint.Status == (int)Status.Backlog)
            {
                var validDate = DateTime.UtcNow - complaint.OpenedDate;
                if (validDate!.Value.TotalDays > 6)
                {
                    throw new WelfareTrackerException("Cannot update complaint status after 7 days of opening.");
                }
                if(leaderStatusVm.Status == (int)Status.InProgress)
                {
                    complaint.Status = leaderStatusVm.Status;
                    await _complaintRepository.UpdateComplaintAsync(complaint);
                    var newStatus = new ComplaintStatus
                    {
                        ComplaintId = leaderStatusVm.ComplaintId,
                        Status = leaderStatusVm.Status,
                        AttemptNumber = complaint.Attempts,
                        DeadlineDate = complaintStatus.DeadlineDate,
                        DateCreated = DateTime.UtcNow
                    };
                    await _complaintStatusRepository.AddComplaintStatusAsync(newStatus);
                    complaint.Status = leaderStatusVm.Status;
                    await _complaintRepository.UpdateComplaintAsync(complaint);
                    await _complaintService.UpdateStatusOfReferredComplaintsAsync(leaderStatusVm.ComplaintId);
                }
                else
                {
                    throw new WelfareTrackerException("Invalid status update from Backlog.");
                }
            }

            else
            {
                if(leaderStatusVm.Status == (int)Status.Resolved)
                {
                    complaint.Status = leaderStatusVm.Status;
                    await _complaintRepository.UpdateComplaintAsync(complaint);
                    var newStatus = new ComplaintStatus
                    {
                        ComplaintId = leaderStatusVm.ComplaintId,
                        Status = leaderStatusVm.Status,
                        AttemptNumber = complaint.Attempts,
                        DeadlineDate = complaintStatus.DeadlineDate,
                        DateCreated = DateTime.UtcNow
                    };
                    await _complaintStatusRepository.AddComplaintStatusAsync(newStatus);
                    complaint.Status = leaderStatusVm.Status;
                    await _complaintRepository.UpdateComplaintAsync(complaint);
                    await _complaintService.UpdateStatusOfReferredComplaintsAsync(leaderStatusVm.ComplaintId);
                }
                else
                {
                    throw new WelfareTrackerException("Invalid status update from InProgress.");
                }
            }
            var updatedComplaint = await _complaintService.GetComplaintByComplaintIdAsync(leaderStatusVm.ComplaintId);
            return updatedComplaint;
        }
    }
}
