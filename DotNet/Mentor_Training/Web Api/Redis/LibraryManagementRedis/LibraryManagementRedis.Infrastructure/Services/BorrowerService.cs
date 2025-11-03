namespace LibraryManagementRedis.Infrastructure.Services;
public class BorrowerService(IBorrowerRepository repo, IMapper mapper) : IBorrowerService
{
    private readonly IBorrowerRepository _repo = repo;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<BorrowerVm>> GetAllBorrowersAsync()
    {
        var borrowers = await _repo.GetAllAsync();
        return _mapper.Map<IEnumerable<BorrowerVm>>(borrowers);
    }
    public async Task<BorrowerVm?> GetBorrowerByIdAsync(int id)
    {
        var borrower = await _repo.GetByIdAsync(id);
        return _mapper.Map<BorrowerVm?>(borrower);
    }
    public async Task<BorrowerVm> CreateBorrowerAsync(BorrowerDto borrowerDto)
    {
        var entity = _mapper.Map<Borrower>(borrowerDto);
        var created = await _repo.AddAsync(entity);
        return _mapper.Map<BorrowerVm>(created);
    }
    public async Task<bool> BorrowBookAsync(int borrowerId, int bookId)
    {
        return await _repo.BorrowBookAsync(borrowerId, bookId);
    }
    public async Task<bool> ReturnBookAsync(int borrowerId, int bookId)
    {
        return await _repo.ReturnBookAsync(borrowerId, bookId);
    }
}
