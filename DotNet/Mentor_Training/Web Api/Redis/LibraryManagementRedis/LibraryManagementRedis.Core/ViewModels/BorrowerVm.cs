namespace LibraryManagementRedis.Core.ViewModels;
public class BorrowerVm
{
    public int BorrowerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> BorrowedBooks { get; set; } = new();
}
