namespace LibraryApp.Domain.Entities;

public class BorrowRecord
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid BookId { get; private set; }
    public Book Book { get; private set; } = null!;
    public DateTime BorrowedAt { get; private set; }
    public DateTime? ReturnedAt { get; private set; }

    private BorrowRecord() { }

    public BorrowRecord(Book book)
    {
        Book = book ?? throw new ArgumentNullException(nameof(book));
        BookId = book.Id;
        BorrowedAt = DateTime.UtcNow;
    }

    public void MarkAsReturned()
    {
        if (ReturnedAt != null) return;
        ReturnedAt = DateTime.UtcNow;
    }
}