namespace LibraryManagementRedis.Core.Contracts.Services;
public interface IBookService
{
    Task<IEnumerable<BookVm>> GetAllBooksAsync();
    Task<BookVm?> GetBookByIdAsync(int id);
    Task<BookVm> CreateBookAsync(BookDto bookDto);
    Task<BookVm?> UpdateBookAsync(int id, BookDto bookDto);
    Task<bool> DeleteBookAsync(int id);
}