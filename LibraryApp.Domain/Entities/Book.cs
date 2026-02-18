namespace LibraryApp.Domain.Entities;

public class Book
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Title { get; private set; } = null!;
    public string Author { get; private set; } = null!;
    public int Year { get; private set; }
    public string ISBN { get; private set; } = null!;
    public int AvailableCopies { get; private set; }

    private readonly List<BorrowRecord> _borrowHistory = [];

    public IReadOnlyCollection<BorrowRecord> BorrowHistory => _borrowHistory.AsReadOnly();

    private Book() { }

    public Book(string title, string author, int year, string isbn, int copies)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title required");
        if (string.IsNullOrWhiteSpace(author)) throw new ArgumentException("Author required");
        if (year < 1500 || year > DateTime.UtcNow.Year) throw new ArgumentException("Invalid year");
        if (copies < 0) throw new ArgumentException("Copies cannot be negative");

        Title = title;
        Author = author;
        Year = year;
        ISBN = isbn;
        AvailableCopies = copies;
    }

    public void Borrow()
    {
        if (AvailableCopies <= 0)
            throw new InvalidOperationException("No copies available to borrow");

        AvailableCopies--;

        var record = new BorrowRecord(this);
        _borrowHistory.Add(record);
    }

    public void Return()
    {
        var activeBorrow = _borrowHistory.LastOrDefault(b => b.ReturnedAt == null);
        if (activeBorrow == null) return;

        activeBorrow.MarkAsReturned();
        AvailableCopies++;
    }
}

