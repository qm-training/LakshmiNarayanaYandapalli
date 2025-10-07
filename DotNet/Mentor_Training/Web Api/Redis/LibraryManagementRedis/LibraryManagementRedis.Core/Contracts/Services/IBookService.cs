using LibraryManagementRedis.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementRedis.Core.Contracts.Services;
public interface IBookService
{
    Task<IEnumerable<Book>> GetAllBooksAsync();
    Task<Book?> GetBookByIdAsync(int id);
    Task<Book> CreateBookAsync(Book book);
    Task<Book?> UpdateBookAsync(Book book);
    Task<bool> DeleteBookAsync(int id);
}