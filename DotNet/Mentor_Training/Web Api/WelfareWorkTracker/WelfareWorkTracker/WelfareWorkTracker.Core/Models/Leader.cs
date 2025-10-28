namespace WelfareWorkTracker.Core.Models;
public class Leader
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string PartyName { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

}
