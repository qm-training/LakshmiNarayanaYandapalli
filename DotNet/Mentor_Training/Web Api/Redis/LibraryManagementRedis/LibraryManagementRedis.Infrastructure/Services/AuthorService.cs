namespace LibraryManagementRedis.Infrastructure.Services;
public class AuthorService(IAuthorRepository repo, IMapper mapper) : IAuthorService
{
    private readonly IAuthorRepository _repo = repo;
    private readonly IMapper _mapper = mapper;
    public async Task<IEnumerable<AuthorVm>> GetAllAuthorsAsync()
    {
        var authors = await _repo.GetAllAsync();
        return _mapper.Map<IEnumerable<AuthorVm>>(authors);
    }
    public async Task<AuthorVm?> GetAuthorByIdAsync(int id)
    {
        var author = await _repo.GetByIdAsync(id);
        return _mapper.Map<AuthorVm?>(author);
    }
    public async Task<AuthorVm> CreateAuthorAsync(AuthorDto authorDto)
    {
        var entity = _mapper.Map<Author>(authorDto);
        var created = await _repo.AddAsync(entity);
        return _mapper.Map<AuthorVm>(created);
    }
    public async Task<AuthorVm?> UpdateAuthorAsync(int id, AuthorDto authorDto)
    {
        var entity = _mapper.Map<Author>(authorDto);
        entity.AuthorId = id;

        var updated = await _repo.UpdateAsync(entity);
        return _mapper.Map<AuthorVm?>(updated);
    }
    public async Task<bool> DeleteAuthorAsync(int id)
    {
        return await _repo.DeleteAsync(id);
    }
}
