namespace LibraryManagementRedis.Core.Models;
public class Author
{
    public int AuthorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;

    public List<Book> Books { get; set; } = new List<Book>();
}
