using LibraryManagementRedis.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementRedis.Core.Contracts.Repository;
public interface IBorrowerRepository
{
    Task<IEnumerable<Borrower>> GetAllAsync();
    Task<Borrower?> GetByIdAsync(int id);
    Task<Borrower> AddAsync(Borrower borrower);
    Task<bool> BorrowBookAsync(int borrowerId, int bookId);
    Task<bool> ReturnBookAsync(int borrowerId, int bookId);
}
