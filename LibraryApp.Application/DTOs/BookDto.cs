namespace LibraryApp.Application.DTOs;

public class BookDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public int Year { get; set; }

    public string ISBN { get; set; } = string.Empty;

    public int AvailableCopies { get; set; }
}
