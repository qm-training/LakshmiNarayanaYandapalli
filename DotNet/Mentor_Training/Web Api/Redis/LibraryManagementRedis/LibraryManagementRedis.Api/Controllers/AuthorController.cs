namespace LibraryManagementRedis.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorsController(IAuthorService service, ICacheService cache) : ControllerBase
{
    private readonly IAuthorService _service = service;
    private readonly ICacheService _cache = cache;

    private const string AllAuthorsCacheKey = "authors:all";
    private static string AuthorCacheKey(int id) => $"author:{id}";

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var cached = await _cache.GetAsync<IEnumerable<AuthorVm>>(AllAuthorsCacheKey);
        if (cached != null)
            return Ok(cached);

        var authors = await _service.GetAllAuthorsAsync();
        await _cache.SetAsync(AllAuthorsCacheKey, authors);
        return Ok(authors);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var cacheKey = AuthorCacheKey(id);
        var cached = await _cache.GetAsync<AuthorVm>(cacheKey);
        if (cached != null)
            return Ok(cached);

        var author = await _service.GetAuthorByIdAsync(id);
        if (author != null)
            await _cache.SetAsync(cacheKey, author);

        return author == null ? NotFound() : Ok(author);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AuthorDto authorDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _service.CreateAuthorAsync(authorDto);

        await _cache.RemoveAsync(AllAuthorsCacheKey);
        return CreatedAtAction(nameof(GetById), new { id = created.AuthorId }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] AuthorDto authorDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = await _service.UpdateAuthorAsync(id, authorDto);
        if (updated != null)
        {
            await _cache.RemoveAsync(AllAuthorsCacheKey);
            await _cache.RemoveAsync(AuthorCacheKey(id));
        }

        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAuthorAsync(id);
        if (deleted)
        {
            await _cache.RemoveAsync(AllAuthorsCacheKey);
            await _cache.RemoveAsync(AuthorCacheKey(id));
        }

        return deleted ? NoContent() : NotFound();
    }
}
