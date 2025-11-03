namespace LibraryManagementRedis.Core.Dtos;
public class BookDto
{
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public int AuthorId { get; set; }
}
