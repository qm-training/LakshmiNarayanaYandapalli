namespace WelfareWorkTracker.Core.Dtos
{
    public class EmailOutboxDto
    {
        public int EmailOutboxId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string? FromUserMail { get; set; }
        public string ToUserMail { get; set; } = string.Empty;
    }
}
