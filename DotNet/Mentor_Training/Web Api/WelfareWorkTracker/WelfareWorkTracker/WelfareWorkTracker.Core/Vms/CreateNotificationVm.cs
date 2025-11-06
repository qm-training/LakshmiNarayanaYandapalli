namespace WelfareWorkTracker.Core.Vms
{
    public class CreateNotificationVm
    {
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string NotificationType { get; set; } = null!;
        public int ToUserId { get; set; }
        public int? FromUserId { get; set; }
    }
}
