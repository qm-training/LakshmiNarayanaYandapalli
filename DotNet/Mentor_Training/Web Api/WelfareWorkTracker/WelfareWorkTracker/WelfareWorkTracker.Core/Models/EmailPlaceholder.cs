namespace WelfareWorkTracker.Core.Models;
public class EmailPlaceholder
{
    public int Id { get; set; }

    public int EmailHistoryId { get; set; }

    public string PlaceHolderKey { get; set; } = null!;

    public string PlaceHolderValue { get; set; } = null!;

    public DateTime DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

}
