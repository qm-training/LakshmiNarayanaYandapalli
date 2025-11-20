namespace WelfareWorkTracker.Core.Constants;
public static class Constants
{
    public static class EmailTemplates
    {
        public static string NewComplaintCitizen { get; } = "NewComplaintCitizen";
        public static string NewComplaintAdminRep { get; } = "NewComplaintAdminRep";
        public static string ComplaintLeaderApprovalDelay { get; } = "ComplaintLeaderApprovalDelay";
        public static string ComplaintLeaderUnresolved { get; } = "ComplaintLeaderUnresolved";
        public static string ComplaintCitizenUnresolved { get; } = "ComplaintCitizenUnresolved";
        public static string ComplaintLeaderBacklog { get; } = "ComplaintLeaderBacklog";
        public static string ComplaintCitizenBacklog { get; } = "ComplaintCitizenBacklog";
        public static string ComplaintCitizenRejected { get; } = "ComplaintCitizenRejected";
        public static string ComplaintCitizenReopen { get; } = "ComplaintCitizenReopen";
        public static string ComplaintCitizenExisting { get; } = "ComplaintCitizenExisting";
        public static string ComplaintCitizenValid { get; } = "ComplaintCitizenValid";
        public static string ComplaintAdminValid { get; } = "ComplaintAdminValid";
        public static string ComplaintAdminInvalid { get; } = "ComplaintAdminInvalid";
        public static string ComplaintCitizenApproval { get; } = "ComplaintCitizenApproval";
        public static string ComplaintCitizenResolved { get; } = "ComplaintCitizenResolved";
        public static string ComplaintConstituencyResolved { get; } = "ComplaintConstituencyResolved";
        public static string DailyComplaintLeaderUnresolved { get; } = "DailyComplaintLeaderUnresolved";
        public static string ComplaintLeaderClosed { get; } = "ComplaintLeaderClosed";
        public static string ComplaintCitizenClosed { get; } = "ComplaintCitizenClosed";
        public static string LeaderReputationIncrease { get; } = "LeaderReputationIncrease";
        public static string LeaderReputationDecrease { get; } = "LeaderReputationDecrease";
        public static string ComplaintLeaderReopened { get; } = "ComplaintLeaderReopened";
    }
}
