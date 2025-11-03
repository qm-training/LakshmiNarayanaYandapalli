namespace LibraryManagementRedis.Core.Contracts.Services;
public interface IBorrowerService
{
    Task<IEnumerable<BorrowerVm>> GetAllBorrowersAsync();
    Task<BorrowerVm?> GetBorrowerByIdAsync(int id);
    Task<BorrowerVm> CreateBorrowerAsync(BorrowerDto borrowerDto);
    Task<bool> BorrowBookAsync(int borrowerId, int bookId);
    Task<bool> ReturnBookAsync(int borrowerId, int bookId);
}
