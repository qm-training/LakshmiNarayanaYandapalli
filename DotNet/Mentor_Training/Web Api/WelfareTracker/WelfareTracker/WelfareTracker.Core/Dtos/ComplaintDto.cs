namespace WelfareTracker.Core.Dtos
{
    public class ComplaintDto
    {
        public int ComplaintId { get; set; }

        public string Description { get; set; } = null!;

        public int ConstituencyId { get; set; }

        public int CitizenId { get; set; }

        public int LeaderId { get; set; }

        public int Attempts { get; set; }

        public bool IsClosed { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }
        public string Status { get; set; } = null!;
    }
}
