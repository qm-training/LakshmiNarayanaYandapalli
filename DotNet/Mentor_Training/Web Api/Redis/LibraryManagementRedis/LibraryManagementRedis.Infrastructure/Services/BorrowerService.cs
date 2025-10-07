using LibraryManagementRedis.Core.Contracts.Repository;
using LibraryManagementRedis.Core.Contracts.Services;
using LibraryManagementRedis.Core.Models;

namespace LibraryManagementRedis.Infrastructure.Services;
public class BorrowerService(IBorrowerRepository repo) : IBorrowerService
{
    private readonly IBorrowerRepository _repo = repo;

    public Task<IEnumerable<Borrower>> GetAllBorrowersAsync() => _repo.GetAllAsync();
    public Task<Borrower?> GetBorrowerByIdAsync(int id) => _repo.GetByIdAsync(id);
    public Task<Borrower> CreateBorrowerAsync(Borrower borrower) => _repo.AddAsync(borrower);
    public Task<bool> BorrowBookAsync(int borrowerId, int bookId) => _repo.BorrowBookAsync(borrowerId, bookId);
    public Task<bool> ReturnBookAsync(int borrowerId, int bookId) => _repo.ReturnBookAsync(borrowerId, bookId);
}
