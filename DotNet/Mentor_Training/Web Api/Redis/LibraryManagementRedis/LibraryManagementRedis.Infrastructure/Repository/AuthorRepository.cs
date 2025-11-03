namespace LibraryManagementRedis.Infrastructure.Repository;
public class AuthorRepository(LibraryDbContext context) : IAuthorRepository
{
    private readonly LibraryDbContext _context = context;

    public async Task<IEnumerable<Author>> GetAllAsync()
    {
        return await _context.Authors
                             .Include(a => a.Books)
                             .ToListAsync();
    }

    public async Task<Author?> GetByIdAsync(int id)
    {
        return await _context.Authors
                             .Include(a => a.Books)
                             .FirstOrDefaultAsync(a => a.AuthorId == id);
    }

    public async Task<Author> AddAsync(Author author)
    {
        _context.Authors.Add(author);
        await _context.SaveChangesAsync();
        return author;
    }

    public async Task<Author?> UpdateAsync(Author author)
    {
        var existing = await _context.Authors.FindAsync(author.AuthorId);
        if (existing == null) return null;

        existing.Name = author.Name;
        existing.Nationality = author.Nationality;
        await _context.SaveChangesAsync();

        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var author = await _context.Authors.FindAsync(id);
        if (author == null) return false;

        _context.Authors.Remove(author);
        await _context.SaveChangesAsync();
        return true;
    }
}
