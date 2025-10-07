namespace LibraryManagementRedis.Core.Models;
public class Book
{
    public int BookId { get; set; }
    public string Title { get; set; }
    public string ISBN { get; set; }
    public bool IsAvailable { get; set; } = true;

    public int AuthorId { get; set; }
    public Author? Author { get; set; }

    public List<Borrower> Borrowers { get; set; } = new List<Borrower>();
}
