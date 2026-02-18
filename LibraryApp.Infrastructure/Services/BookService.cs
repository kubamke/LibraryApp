namespace LibraryApp.Infrastructure.Services;

using LibraryApp.Application.DTOs;
using LibraryApp.Application.Interfaces;
using LibraryApp.Application.Services;
using LibraryApp.Domain.Entities;
using LibraryApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

/// <summary>
/// Service responsible for managing books and their borrowing lifecycle.
/// Uses Entity Framework Core for data persistence.
/// </summary>
public class BookService(AppDbContext context, ILogger<BookService> logger) : IBookService
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<BookService> _logger = logger;

    /// <inheritdoc />
    public async Task<List<BookDto>> GetAllAsync()
    {
        var books = await _context.Books.ToListAsync();
        return [.. books.Select(b => b.ToDto())];
    }

    /// <inheritdoc />
    public async Task<BookDto?> GetByIdAsync(Guid id)
    {
        var book = await _context.Books.FindAsync(id);
        return book?.ToDto();
    }

    /// <inheritdoc />
    public async Task<List<BookDto>> SearchAsync(string query)
    {
        var books = await _context.Books
            .Where(b =>
                b.Title.Contains(query) ||
                b.Author.Contains(query) ||
                b.ISBN.Contains(query))
            .ToListAsync();

        return [.. books.Select(b => b.ToDto())];
    }

    /// <inheritdoc />
    public async Task<Guid> AddAsync(CreateBookDto dto)
    {
        var book = new Book(
            dto.Title,
            dto.Author,
            dto.Year,
            dto.ISBN,
            dto.Copies);

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Book added: {Title}", dto.Title);

        return book.Id;
    }

    /// <inheritdoc />
    public async Task BorrowAsync(Guid id)
    {
        var book = await _context.Books
            .Include(b => b.BorrowHistory)
            .FirstOrDefaultAsync(b => b.Id == id)
            ?? throw new KeyNotFoundException("Book not found");

        try
        {
            book.Borrow();
        }
        catch (InvalidOperationException)
        {
            throw;
        }

        var newRecord = book.BorrowHistory.Last();
        _context.BorrowRecords.Add(newRecord);

        await _context.SaveChangesAsync();

        _logger.LogInformation("Book borrowed: {BookId}", id);
    }
    
    /// <inheritdoc />
    public async Task ReturnAsync(Guid id)
    {
        var book = await _context.Books
            .Include(b => b.BorrowHistory)
            .FirstOrDefaultAsync(b => b.Id == id)
            ?? throw new KeyNotFoundException("Book not found.");

        book.Return();

        await _context.SaveChangesAsync();

        _logger.LogInformation("Book returned: {BookId}", id);
    }

    /// <inheritdoc />
    public async Task<List<BorrowRecordDto>> GetBorrowHistoryAsync(Guid bookId)
    {
        var book = await _context.Books
            .Include(b => b.BorrowHistory)
            .FirstOrDefaultAsync(b => b.Id == bookId) ?? throw new KeyNotFoundException("Book not found.");

        return [.. book.BorrowHistory
            .Select(r => r.ToDto())
            .OrderByDescending(r => r.BorrowedAt)];
    }
}