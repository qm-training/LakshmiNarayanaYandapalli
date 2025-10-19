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
                var linkedcomplaintStatus = await _complaintStatusRepository.GetComplaintStatusByIdAsync(adminRepStatusVm.ReferenceNumber) ?? throw new WelfareTrackerException("Referenced complaint status not found.");                

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
                    var referredComplaintStatusUpdate = await _complaintService.UpdateStatusOfReferredComplaintsAsync(adminRepStatusVm.ReferenceNumber);
                    if (!referredComplaintStatusUpdate)
                    {
                        throw new WelfareTrackerException("Failed to update referred complaints statuses.");
                    }
                }
            }
            else
            {
                var complaintToUpdate = await _complaintRepository.GetComplaintByIdAsync(adminRepStatusVm.ComplaintId) ?? throw new WelfareTrackerException("Complaint not found.");
                if(!(complaintToUpdate.Status != (int)Status.UnderValidation || complaintToUpdate.Status != (int)Status.Unresolved))
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
    }
}
