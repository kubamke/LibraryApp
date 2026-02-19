namespace LibraryApp.Tests;

using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LibraryApp.Application.DTOs;
using System.ComponentModel;

public class BooksApiTests
{
    [Fact]
    [Description(@"
        Given: A new book DTO
        When: The book is posted to the API
        Then: The response should be Created and the book should exist in the list")]
    public async Task CreateBook_ShouldReturnCreatedBook()
    {
        using var factory = new CustomWebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var createDto = new CreateBookDto
        {
            Title = "Test Book",
            Author = "Test Author",
            Year = 2024,
            ISBN = "1234567890",
            Copies = 5
        };

        var response = await client.PostAsJsonAsync("/api/books", createDto);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var books = await client.GetFromJsonAsync<List<BookDto>>("/api/books");
        books.Should().NotBeNull();
        books!.Count.Should().Be(1);
        books[0].Title.Should().Be("Test Book");
    }

    [Fact]
    [Description(@"
        Given: A book that has not been borrowed
        When: Return is attempted on the book
        Then: The API should return NoContent and the available copies should remain unchanged")]
    public async Task ReturnBook_WhenNotBorrowed_ShouldDoNothing()
    {
        using var factory = new CustomWebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var createDto = new CreateBookDto
        {
            Title = "Return Nothing Test",
            Author = "Author",
            Year = 2024,
            ISBN = "1111111111",
            Copies = 1
        };

        var createResponse = await client.PostAsJsonAsync("/api/books", createDto);
        var location = createResponse.Headers.Location!.ToString();
        var id = location.Split('/').Last();

        var returnResponse = await client.PostAsync($"/api/books/{id}/return", null);
        returnResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var book = await client.GetFromJsonAsync<BookDto>($"/api/books/{id}");
        book!.AvailableCopies.Should().Be(1);
    }

    [Fact]
    [Description(@"
        Given: A book with one available copy
        When: The book is borrowed
        Then: The available copies should decrease by one")]
    public async Task BorrowBook_ShouldDecreaseCopies()
    {
        using var factory = new CustomWebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var createDto = new CreateBookDto
        {
            Title = "Borrow Test",
            Author = "Author",
            Year = 2024,
            ISBN = "1234567890123",
            Copies = 1
        };

        var createResponse = await client.PostAsJsonAsync("/api/books", createDto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var location = createResponse.Headers.Location!.ToString();
        var id = location.Split('/').Last();

        var borrowResponse = await client.PostAsync($"/api/books/{id}/borrow", null);
        borrowResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var book = await client.GetFromJsonAsync<BookDto>($"/api/books/{id}");
        book!.AvailableCopies.Should().Be(0);
    }

    [Fact]
    [Description(@"
        Given: A book with zero available copies
        When: Borrow is attempted
        Then: The API should return BadRequest")]
    public async Task BorrowBook_WhenNoCopies_ShouldThrow()
    {
        using var factory = new CustomWebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var createDto = new CreateBookDto
        {
            Title = "Borrow Fail Test",
            Author = "Author",
            Year = 2024,
            ISBN = "0000000000",
            Copies = 0
        };

        var createResponse = await client.PostAsJsonAsync("/api/books", createDto);
        var location = createResponse.Headers.Location!.ToString();
        var id = location.Split('/').Last();

        var borrowResponse = await client.PostAsync($"/api/books/{id}/borrow", null);
        borrowResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    [Description(@"
        Given: A book that has been borrowed
        When: The book is returned
        Then: The available copies should increase by one")]
    public async Task ReturnBook_ShouldIncreaseCopies()
    {
        using var factory = new CustomWebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var createDto = new CreateBookDto
        {
            Title = "Return Test",
            Author = "Author",
            Year = 2024,
            ISBN = "0987654321",
            Copies = 1
        };

        var createResponse = await client.PostAsJsonAsync("/api/books", createDto);
        var location = createResponse.Headers.Location!.ToString();
        var id = location.Split('/').Last();

        var borrowResponse = await client.PostAsync($"/api/books/{id}/borrow", null);
        borrowResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var returnResponse = await client.PostAsync($"/api/books/{id}/return", null);
        returnResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var book = await client.GetFromJsonAsync<BookDto>($"/api/books/{id}");
        book!.AvailableCopies.Should().Be(1);
    }

    [Fact]
    [Description(@"
        Given: Multiple books in the system
        When: Searching by title, author, or ISBN
        Then: The search should return only matching books")]
    public async Task SearchBooks_ShouldReturnMatches()
    {
        using var factory = new CustomWebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var booksToCreate = new[]
        {
            new CreateBookDto { Title = "C# in Depth", Author = "Jon Skeet", Year = 2021, ISBN = "1111111111", Copies = 1 },
            new CreateBookDto { Title = "Effective C#", Author = "Bill Wagner", Year = 2020, ISBN = "0987654321", Copies = 2 },
            new CreateBookDto { Title = "Python 101", Author = "Someone", Year = 2022, ISBN = "0000000000", Copies = 1 },
        };

        foreach (var dto in booksToCreate)
        {
            var createResponse = await client.PostAsJsonAsync("/api/books", dto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        // Search by title
        var result = await client.GetFromJsonAsync<List<BookDto>>("/api/books/search?q=C#");
        result!.Count.Should().Be(2);

        // Search by author
        var result2 = await client.GetFromJsonAsync<List<BookDto>>("/api/books/search?q=Someone");
        result2!.Count.Should().Be(1);

        // Search by ISBN
        var result3 = await client.GetFromJsonAsync<List<BookDto>>("/api/books/search?q=0987654321");
        result3!.Count.Should().Be(1);
    }

    [Fact]
    [Description(@"
        Given: A book with no borrow records
        When: The history endpoint is requested
        Then: The API should return an empty list")]
    public async Task GetBorrowHistory_WhenNoBorrows_ShouldReturnEmpty()
    {
        using var factory = new CustomWebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var createDto = new CreateBookDto
        {
            Title = "History Empty Test",
            Author = "Author",
            Year = 2024,
            ISBN = "9000000001",
            Copies = 2
        };

        var createResponse = await client.PostAsJsonAsync("/api/books", createDto);
        var id = createResponse.Headers.Location!.ToString().Split('/').Last();

        var history = await client.GetFromJsonAsync<List<BorrowRecordDto>>($"/api/books/{id}/history");

        history.Should().NotBeNull();
        history!.Count.Should().Be(0);
    }

    [Fact]
    [Description(@"
        Given: A book that has been borrowed once
        When: The history endpoint is requested
        Then: The API should return one active borrow record with no return date")]
    public async Task GetBorrowHistory_AfterBorrow_ShouldReturnActiveRecord()
    {
        using var factory = new CustomWebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var createDto = new CreateBookDto
        {
            Title = "History Borrow Test",
            Author = "Author",
            Year = 2024,
            ISBN = "9000000002",
            Copies = 1
        };

        var createResponse = await client.PostAsJsonAsync("/api/books", createDto);
        var id = createResponse.Headers.Location!.ToString().Split('/').Last();

        await client.PostAsync($"/api/books/{id}/borrow", null);

        var history = await client.GetFromJsonAsync<List<BorrowRecordDto>>($"/api/books/{id}/history");

        history.Should().NotBeNull();
        history!.Count.Should().Be(1);
        history[0].ReturnedAt.Should().BeNull();
    }

    [Fact]
    [Description(@"
        Given: A book that has been borrowed and returned
        When: The history endpoint is requested
        Then: The API should return one borrow record with a return date")]
    public async Task GetBorrowHistory_AfterReturn_ShouldShowReturnedRecord()
    {
        using var factory = new CustomWebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var createDto = new CreateBookDto
        {
            Title = "History Return Test",
            Author = "Author",
            Year = 2024,
            ISBN = "9000000003",
            Copies = 1
        };

        var createResponse = await client.PostAsJsonAsync("/api/books", createDto);
        var id = createResponse.Headers.Location!.ToString().Split('/').Last();

        await client.PostAsync($"/api/books/{id}/borrow", null);
        await client.PostAsync($"/api/books/{id}/return", null);

        var history = await client.GetFromJsonAsync<List<BorrowRecordDto>>($"/api/books/{id}/history");

        history.Should().NotBeNull();
        history!.Count.Should().Be(1);
        history[0].ReturnedAt.Should().NotBeNull();
    }

    [Fact]
    [Description(@"
        Given: A book that has been borrowed and returned multiple times
        When: The history endpoint is requested
        Then: The API should return all borrow records in the system")]
    public async Task GetBorrowHistory_MultipleBorrows_ShouldReturnAllRecords()
    {
        using var factory = new CustomWebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var createDto = new CreateBookDto
        {
            Title = "History Multiple Test",
            Author = "Author",
            Year = 2024,
            ISBN = "9000000004",
            Copies = 1
        };

        var createResponse = await client.PostAsJsonAsync("/api/books", createDto);
        var id = createResponse.Headers.Location!.ToString().Split('/').Last();

        await client.PostAsync($"/api/books/{id}/borrow", null);
        await client.PostAsync($"/api/books/{id}/return", null);

        await client.PostAsync($"/api/books/{id}/borrow", null);
        await client.PostAsync($"/api/books/{id}/return", null);

        var history = await client.GetFromJsonAsync<List<BorrowRecordDto>>($"/api/books/{id}/history");

        history.Should().NotBeNull();
        history!.Count.Should().Be(2);
        history.All(h => h.ReturnedAt != null).Should().BeTrue();
    }

    [Fact]
    [Description(@"
        Given: A book ID that does not exist
        When: The history endpoint is requested
        Then: The API should return NotFound")]
    public async Task GetBorrowHistory_NonexistentBook_ShouldReturn404()
    {
        using var factory = new CustomWebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/books/{Guid.NewGuid()}/history");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
