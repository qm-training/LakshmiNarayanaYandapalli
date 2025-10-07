using LibraryManagementRedis.Core.Contracts.Caching;
using LibraryManagementRedis.Core.Contracts.Services;
using LibraryManagementRedis.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementRedis.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class BorrowersController(IBorrowerService service, ICacheService cache) : ControllerBase
{
    private readonly IBorrowerService _service = service;
    private readonly ICacheService _cache = cache;

    private const string AllBorrowersCacheKey = "borrowers:all";
    private static string BorrowerCacheKey(int id) => $"borrower:{id}";

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var cached = await _cache.GetAsync<IEnumerable<Borrower>>(AllBorrowersCacheKey);
        if (cached != null)
            return Ok(cached);

        var borrowers = await _service.GetAllBorrowersAsync();
        await _cache.SetAsync(AllBorrowersCacheKey, borrowers);
        return Ok(borrowers);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var cacheKey = BorrowerCacheKey(id);
        var cached = await _cache.GetAsync<Borrower>(cacheKey);
        if (cached != null)
            return Ok(cached);

        var borrower = await _service.GetBorrowerByIdAsync(id);
        if (borrower != null)
            await _cache.SetAsync(cacheKey, borrower);

        return borrower == null ? NotFound() : Ok(borrower);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Borrower borrower)
    {
        var created = await _service.CreateBorrowerAsync(borrower);
        await _cache.RemoveAsync(AllBorrowersCacheKey);
        return CreatedAtAction(nameof(GetById), new { id = created.BorrowerId }, created);
    }

    [HttpPost("{borrowerId}/borrow/{bookId}")]
    public async Task<IActionResult> BorrowBook(int borrowerId, int bookId)
    {
        var result = await _service.BorrowBookAsync(borrowerId, bookId);
        if (result)
        {
            await _cache.RemoveAsync(AllBorrowersCacheKey);
            await _cache.RemoveAsync(BorrowerCacheKey(borrowerId));
        }
        return result ? Ok("Book borrowed successfully") : BadRequest("Book unavailable or borrower not found");
    }

    [HttpPost("{borrowerId}/return/{bookId}")]
    public async Task<IActionResult> ReturnBook(int borrowerId, int bookId)
    {
        var result = await _service.ReturnBookAsync(borrowerId, bookId);
        if (result)
        {
            await _cache.RemoveAsync(AllBorrowersCacheKey);
            await _cache.RemoveAsync(BorrowerCacheKey(borrowerId));
        }
        return result ? Ok("Book returned successfully") : BadRequest("Invalid borrower or book");
    }
}
