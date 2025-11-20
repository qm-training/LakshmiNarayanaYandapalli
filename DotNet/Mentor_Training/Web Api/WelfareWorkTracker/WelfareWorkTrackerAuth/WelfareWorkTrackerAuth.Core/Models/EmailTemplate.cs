namespace WelfareWorkTrackerAuth.Core.Models;
public class EmailTemplate
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Subject { get; set; } = null!;

    public string Body { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public int CreatedBy { get; set; }

}
