namespace LibraryManagementRedis.Core.Contracts.Services;
public interface IAuthorService
{
    Task<IEnumerable<AuthorVm>> GetAllAuthorsAsync();
    Task<AuthorVm?> GetAuthorByIdAsync(int id);
    Task<AuthorVm> CreateAuthorAsync(AuthorDto authorDto);
    Task<AuthorVm?> UpdateAuthorAsync(int id, AuthorDto authorDto);
    Task<bool> DeleteAuthorAsync(int id);
}
