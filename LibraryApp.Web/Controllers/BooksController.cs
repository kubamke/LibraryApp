namespace LibraryApp.Web.Controllers;

using LibraryApp.Application.DTOs;
using LibraryApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiExplorerSettings(IgnoreApi = true)]
[Route("books")]
public class BooksController(IBookService service) : Controller
{
    private readonly IBookService _service = service;

    // GET /books
    [HttpGet("")]
    public async Task<IActionResult> Index(string? query)
    {
        IEnumerable<BookDto> books = string.IsNullOrWhiteSpace(query)
            ? await _service.GetAllAsync()
            : await _service.SearchAsync(query);

        ViewBag.Query = query;
        return View(books);
    }

    // GET /books/create
    [HttpGet("create")]
    public IActionResult Create()
    {
        return View();
    }

    // POST /books/create
    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateBookDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        await _service.AddAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    // POST /books/{id}/borrow
    [HttpPost("{id}/borrow")]
    public async Task<IActionResult> Borrow(Guid id)
    {
        try
        {
            await _service.BorrowAsync(id);
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    // POST /books/{id}/return
    [HttpPost("{id}/return")]
    public async Task<IActionResult> Return(Guid id)
    {
        await _service.ReturnAsync(id);
        return RedirectToAction(nameof(Index));
    }

    // GET /books/{id}/history
    [HttpGet("{id}/history")]
    public async Task<IActionResult> History(Guid id)
    {
        var history = await _service.GetBorrowHistoryAsync(id);
        ViewBag.BookId = id;
        return View(history);
    }
}
