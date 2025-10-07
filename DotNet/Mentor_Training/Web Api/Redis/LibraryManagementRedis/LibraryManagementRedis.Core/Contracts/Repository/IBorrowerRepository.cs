namespace LibraryManagementRedis.Core.Contracts.Repository;
public interface IBorrowerRepository
{
    Task<IEnumerable<Borrower>> GetAllAsync();
    Task<Borrower?> GetByIdAsync(int id);
    Task<Borrower> AddAsync(Borrower borrower);
    Task<bool> BorrowBookAsync(int borrowerId, int bookId);
    Task<bool> ReturnBookAsync(int borrowerId, int bookId);
}
