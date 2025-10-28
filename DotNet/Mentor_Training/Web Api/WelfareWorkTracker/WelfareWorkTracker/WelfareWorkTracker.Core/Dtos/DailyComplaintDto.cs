﻿namespace WelfareWorkTracker.Core.Dtos
{
    public class DailyComplaintDto
    {
        public int DailyComplaintId { get; set; }

        public int ConstituencyId { get; set; }

        public int LeaderId { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }
    }
}
