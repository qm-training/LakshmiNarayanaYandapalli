﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WelfareTracker.Core.Models;

namespace WelfareTracker.Core.Contracts.Repository
{
    public interface IFeedbackRepository
    {
        Task<ComplaintFeedback> AddFeedbackAsync(ComplaintFeedback citizenFeedback);
        Task<ComplaintFeedback?> GetFeedbackByUserAsync(int userId, int? complaintId, int? dailyComplaintId);
        Task<List<ComplaintFeedback>> GetAllFeedbacksAsync(int? complaintId, int? dailyComplaintId);
        Task<int> GetSatisfiedCount(int? complaintId = null, int? dailyComplaintId = null);
        Task<int> GetUnSatisfiedCount(int? complaintId = null, int? dailyComplaintId = null);
    }
}
