namespace WelfareWorkTracker.Core.Vms
{
    public class CreateEmailTemplateVm
    {
        public string Name { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}
