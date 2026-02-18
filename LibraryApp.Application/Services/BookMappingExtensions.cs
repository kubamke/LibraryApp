namespace LibraryApp.Application.Services;

using LibraryApp.Application.DTOs;
using LibraryApp.Domain.Entities;

public static class BookMappingExtensions
{
    public static BookDto ToDto(this Book book)
    {
        return new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Year = book.Year,
            ISBN = book.ISBN,
            AvailableCopies = book.AvailableCopies
        };
    }
}
