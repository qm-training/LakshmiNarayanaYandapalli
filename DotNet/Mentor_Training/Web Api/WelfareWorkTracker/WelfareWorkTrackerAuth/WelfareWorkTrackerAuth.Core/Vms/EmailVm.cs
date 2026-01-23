namespace WelfareWorkTrackerAuth.Core.Vms;

public class EmailVm
{
    public int TemplateId { get; set; }
    public string ToUserEmail { get; set; } = string.Empty;
    public string? FromUserEmail { get; set; }
    public Dictionary<string, string> Payload { get; set; } = new Dictionary<string, string>();
}
