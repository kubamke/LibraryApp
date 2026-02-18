namespace LibraryApp.Application.Interfaces;

using LibraryApp.Application.DTOs;

/// <summary>
/// Provides operations for managing books and their borrowing lifecycle.
/// </summary>
public interface IBookService
{
    /// <summary>
    /// Retrieves all books in the system.
    /// </summary>
    /// <returns>A list of all books.</returns>
    Task<List<BookDto>> GetAllAsync();

    /// <summary>
    /// Retrieves a single book by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the book.</param>
    /// <returns>The book if found; otherwise null.</returns>
    Task<BookDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// Searches books by title, author, or ISBN.
    /// </summary>
    /// <param name="query">The search term.</param>
    /// <returns>A list of matching books.</returns>
    Task<List<BookDto>> SearchAsync(string query);

    /// <summary>
    /// Adds a new book to the system.
    /// </summary>
    /// <param name="dto">The book creation data.</param>
    /// <returns>The unique identifier of the created book.</returns>
    Task<Guid> AddAsync(CreateBookDto dto);

    /// <summary>
    /// Borrows a book, decreasing its available copies.
    /// </summary>
    /// <param name="id">The unique identifier of the book.</param>
    Task BorrowAsync(Guid id);

    /// <summary>
    /// Returns a borrowed book, increasing its available copies.
    /// </summary>
    /// <param name="id">The unique identifier of the book.</param>
    Task ReturnAsync(Guid id);

    /// <summary>
    /// Retrieves the borrowing history of a specific book.
    /// </summary>
    /// <param name="bookId">The unique identifier of the book.</param>
    /// <returns>A list of borrow records ordered by most recent first.</returns>
    Task<List<BorrowRecordDto>> GetBorrowHistoryAsync(Guid bookId);
}
