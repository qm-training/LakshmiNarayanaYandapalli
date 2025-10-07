using LibraryManagementRedis.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementRedis.Infrastructure.Data;
public class LibraryDbContext: DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
    : base(options)
    {
    }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Borrower> Borrowers => Set<Borrower>();

}
