using LibraryManagementRedis.Core.Contracts.Caching;
using LibraryManagementRedis.Core.Contracts.Services;
using LibraryManagementRedis.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementRedis.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController(IBookService service, ICacheService cache) : ControllerBase
{
    private readonly IBookService _service = service;
    private readonly ICacheService _cache = cache;

    private const string AllBooksCacheKey = "books:all";
    private static string BookCacheKey(int id) => $"book:{id}";

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {

        var cached = await _cache.GetAsync<IEnumerable<Book>>(AllBooksCacheKey);
        if (cached != null)
            return Ok(cached);

        var books = await _service.GetAllBooksAsync();
        await _cache.SetAsync(AllBooksCacheKey, books);
        return Ok(books);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var cacheKey = BookCacheKey(id);
        var cached = await _cache.GetAsync<Book>(cacheKey);
        if (cached != null)
            return Ok(cached);

        var book = await _service.GetBookByIdAsync(id);

        if (book != null)
            await _cache.SetAsync(cacheKey, book);

        return book == null ? NotFound() : Ok(book);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Book book)
    {
        var created = await _service.CreateBookAsync(book);
        await _cache.RemoveAsync(AllBooksCacheKey);
        return CreatedAtAction(nameof(GetById), new { id = created.BookId }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Book book)
    {
        if (id != book.BookId) return BadRequest();
        var updated = await _service.UpdateBookAsync(book);

        if (updated != null)
        {
            await _cache.RemoveAsync(AllBooksCacheKey);
            await _cache.RemoveAsync(BookCacheKey(book.BookId));
        }

        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteBookAsync(id);

        if (deleted)
        {
            await _cache.RemoveAsync(AllBooksCacheKey);
            await _cache.RemoveAsync(BookCacheKey(id));
        }

        return deleted ? NoContent() : NotFound();
    }
}