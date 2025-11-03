namespace LibraryManagementRedis.Infrastructure.Repository;
public class BorrowerRepository(LibraryDbContext context) : IBorrowerRepository
{
    private readonly LibraryDbContext _context = context;

    public async Task<IEnumerable<Borrower>> GetAllAsync()
    {
        return await _context.Borrowers
                                .Include(b => b.BorrowedBooks)
                                .ToListAsync();
    }

    public async Task<Borrower?> GetByIdAsync(int id)
    {
        return await _context.Borrowers
                                .Include(b => b.BorrowedBooks)
                                .FirstOrDefaultAsync(b => b.BorrowerId == id);
    }

    public async Task<Borrower> AddAsync(Borrower borrower)
    {
        _context.Borrowers.Add(borrower);
        await _context.SaveChangesAsync();
        return borrower;
    }

    public async Task<bool> BorrowBookAsync(int borrowerId, int bookId)
    {
        var borrower = await _context.Borrowers
                                        .Include(b => b.BorrowedBooks)
                                        .FirstOrDefaultAsync(b => b.BorrowerId == borrowerId);
        var book = await _context.Books.FindAsync(bookId);

        if (borrower == null || book == null || !book.IsAvailable) return false;

        borrower.BorrowedBooks.Add(book);
        book.IsAvailable = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReturnBookAsync(int borrowerId, int bookId)
    {
        var borrower = await _context.Borrowers.Include(b => b.BorrowedBooks)
                                               .FirstOrDefaultAsync(b => b.BorrowerId == borrowerId);
        var book = await _context.Books.FindAsync(bookId);

        if (borrower == null || book == null) return false;

        borrower.BorrowedBooks.Remove(book);
        book.IsAvailable = true;
        await _context.SaveChangesAsync();
        return true;
    }
}
