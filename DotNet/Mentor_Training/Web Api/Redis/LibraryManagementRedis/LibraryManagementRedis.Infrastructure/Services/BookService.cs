using LibraryManagementRedis.Core.Contracts.Repository;
using LibraryManagementRedis.Core.Contracts.Services;
using LibraryManagementRedis.Core.Models;

namespace LibraryManagementRedis.Infrastructure.Services;
public class BookService(IBookRepository repo) : IBookService
{
    private readonly IBookRepository _repo = repo;
    public Task<IEnumerable<Book>> GetAllBooksAsync() => _repo.GetAllAsync();
    public Task<Book?> GetBookByIdAsync(int id) => _repo.GetByIdAsync(id);
    public Task<Book> CreateBookAsync(Book book) => _repo.AddAsync(book);
    public Task<Book?> UpdateBookAsync(Book book) => _repo.UpdateAsync(book);
    public Task<bool> DeleteBookAsync(int id) => _repo.DeleteAsync(id);
}
