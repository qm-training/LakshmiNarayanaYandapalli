using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WelfareTracker.Core.Vms
{
    public class FeedbackVm
    {
        public string? FeedbackMessage { get; set; }

        public bool IsSatisfied { get; set; }

        public int? ComplaintId { get; set; }

        public int? DailyComplaintId { get; set; }
    }
}
