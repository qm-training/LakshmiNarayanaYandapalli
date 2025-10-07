using AutoMapper;
using LibraryManagementRedis.Core.Contracts.Repository;
using LibraryManagementRedis.Core.Contracts.Services;
using LibraryManagementRedis.Core.Dtos;
using LibraryManagementRedis.Core.Models;
using LibraryManagementRedis.Core.ViewModels;

namespace LibraryManagementRedis.Infrastructure.Services;
public class BookService(IBookRepository _repo, IAuthorRepository _authorRepo, IMapper _mapper) : IBookService
{
    public async Task<IEnumerable<BookVm>> GetAllBooksAsync()
    {
        var books = await _repo.GetAllAsync();
        return _mapper.Map<IEnumerable<BookVm>>(books);
    }
    public async Task<BookVm?> GetBookByIdAsync(int id)
    {
        var book = await _repo.GetByIdAsync(id);
        return _mapper.Map<BookVm?>(book);
    }
    public async Task<BookVm> CreateBookAsync(BookDto bookDto)
    {
        var author = await _authorRepo.GetByIdAsync(bookDto.AuthorId);
        if (author == null)
        {
            throw new InvalidOperationException(
                $"Author with ID {bookDto.AuthorId} does not exist. Please add the author first before creating a book.");
        }

        var entity = _mapper.Map<Book>(bookDto);
        var created = await _repo.AddAsync(entity);

        return _mapper.Map<BookVm>(created);
    }
    public async Task<BookVm?> UpdateBookAsync(int id, BookDto bookDto)
    {
        var bookEntity = _mapper.Map<Book>(bookDto);
        bookEntity.BookId = id;

        var updatedBook = await _repo.UpdateAsync(bookEntity);
        return _mapper.Map<BookVm?>(updatedBook);
    }

    public async Task<bool> DeleteBookAsync(int id)
    {
        return await _repo.DeleteAsync(id);
    }
}
