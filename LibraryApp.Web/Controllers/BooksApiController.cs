namespace LibraryApp.Web.Controllers;

using LibraryApp.Application.DTOs;
using LibraryApp.Application.Interfaces;
using LibraryApp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// API endpoints for managing books and their borrowing operations.
/// </summary>
[ApiController]
[Route("api/books")]
public class BooksApiController(IBookService service) : ControllerBase
{
    private readonly IBookService _service = service;

    /// <summary>
    /// Retrieves all books.
    /// </summary>
    /// <returns>A list of books.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllAsync());

    /// <summary>
    /// Retrieves a specific book by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the book.</param>
    /// <returns>The book if found; otherwise 404.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var book = await _service.GetByIdAsync(id);
        return book == null ? NotFound() : Ok(book);
    }

    /// <summary>
    /// Searches books by title, author, or ISBN.
    /// </summary>
    /// <param name="q">Search query.</param>
    /// <returns>A list of matching books.</returns>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q)
        => Ok(await _service.SearchAsync(q));

    /// <summary>
    /// Creates a new book.
    /// </summary>
    /// <param name="dto">Book creation data.</param>
    /// <returns>201 Created with location header.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookDto dto)
    {
        var id = await _service.AddAsync(dto);
        return CreatedAtAction(nameof(Get), new { id }, null);
    }

    /// <summary>
    /// Attempts to borrow a book by decreasing its available copies.
    /// If the book has no copies left, the operation fails and returns a 400 Bad Request.
    /// </summary>
    /// <param name="id">The unique identifier of the book to borrow.</param>
    /// <returns>
    /// 204 No Content if the borrow succeeds;
    /// 400 Bad Request if no copies are available or the operation cannot be performed.
    /// </returns>
    [HttpPost("{id}/borrow")]
    public async Task<IActionResult> Borrow(Guid id)
    {
        try
        {
            await _service.BorrowAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Returns a borrowed book, increasing its available copies.
    /// </summary>
    /// <param name="id">The unique identifier of the book.</param>
    /// <returns>204 No Content on success.</returns>
    [HttpPost("{id}/return")]
    public async Task<IActionResult> Return(Guid id)
    {
        await _service.ReturnAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Retrieves the borrowing history for a specific book.
    /// </summary>
    /// <param name="id">The unique identifier of the book.</param>
    /// <returns>A list of borrow records or 404 if not found.</returns>
    [HttpGet("{id}/history")]
    public async Task<IActionResult> GetBorrowHistory(Guid id)
    {
        try
        {
            var history = await _service.GetBorrowHistoryAsync(id);
            return Ok(history);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
