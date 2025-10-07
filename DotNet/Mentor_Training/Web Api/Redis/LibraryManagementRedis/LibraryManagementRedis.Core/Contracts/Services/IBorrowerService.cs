using LibraryManagementRedis.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementRedis.Core.Contracts.Services;
public interface IBorrowerService
{
    Task<IEnumerable<Borrower>> GetAllBorrowersAsync();
    Task<Borrower?> GetBorrowerByIdAsync(int id);
    Task<Borrower> CreateBorrowerAsync(Borrower borrower);
    Task<bool> BorrowBookAsync(int borrowerId, int bookId);
    Task<bool> ReturnBookAsync(int borrowerId, int bookId);
}
