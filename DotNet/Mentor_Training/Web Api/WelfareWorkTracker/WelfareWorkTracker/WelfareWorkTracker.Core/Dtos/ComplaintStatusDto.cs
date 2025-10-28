using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WelfareWorkTracker.Core.Dtos
{
    public class ComplaintStatusDto
    {
        public int ComplaintStatusId { get; set; }

        public int ComplaintId { get; set; }

        public int Status { get; set; }

        public int AttemptNumber { get; set; }

        public DateTime? DeadlineDate { get; set; }

        public string? RejectReason { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
