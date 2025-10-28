namespace WelfareWorkTracker.Core.Enums
{
    public enum Status
    {
        All = 0,
        UnderValidation = 1,
        Valid = 2,
        Invalid = 3,
        Backlog = 4,
        Reject = 5,
        Approve = 6,
        InProgress = 7,
        Resolved = 8,
        Reopened = 9,
        Unresolved = 10,
        Closed = 11
    }

    public enum RequestStatus
    {
        NotPending = 0,
        Pending,
        Approved,
        Rejected
    }
}
