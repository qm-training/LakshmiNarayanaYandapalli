namespace LibraryManagementRedis.Infrastructure.Repository;
public class BookRepository(LibraryDbContext context) : IBookRepository
{
    private readonly LibraryDbContext _context = context;

    public async Task<IEnumerable<Book>> GetAllAsync()
    {
        return await _context.Books
                             .Include(b => b.Author)
                             .ToListAsync();
    }
    public async Task<Book?> GetByIdAsync(int id)
    {
       return await _context.Books
                            .Include(b => b.Author)
                            .FirstOrDefaultAsync(b => b.BookId == id);
    }
    public async Task<Book> AddAsync(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return book;
    }

    public async Task<Book?> UpdateAsync(Book book)
    {
        var existing = await _context.Books.FindAsync(book.BookId);
        if (existing == null) return null;

        existing.Title = book.Title;
        existing.ISBN = book.ISBN;
        existing.AuthorId = book.AuthorId;
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null) return false;

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return true;
    }
}
