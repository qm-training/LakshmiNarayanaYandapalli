namespace LibraryManagementRedis.Core.ViewModels;
public class BookVm
{
    public int BookId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
}
