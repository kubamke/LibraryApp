# LibraryApp

A .NET 9 Web API + MVC application for basic library management.  
Users can manage books, borrow/return them, and view borrowing history. The app uses SQLite for data persistence and includes a simple web UI and API endpoints.

---

## Features

- Manage books with:
  - `ID` (GUID)
  - `Title`
  - `Author`
  - `Year`
  - `ISBN`
  - `AvailableCopies`
- API & UI support for:
  - Listing all books
  - Adding a new book
  - Searching by title, author, or ISBN
  - Borrowing a book (decreases available copies)
  - Returning a book (increases available copies)
  - Viewing borrowing history
- Input validation:
  - Required fields (`Title`, `Author`, `ISBN`)
  - Year range: 1500–current year
  - ISBN: 10 or 13 Digits
  - 10000 >= Copies <= 0 
- Swagger UI for API testing
- Integration tests for all API endpoints
- Uses Entity Framework Core and SQLite for storage

---

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Optional: Visual Studio 2022+ or VS Code

### Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/kubamke/LibraryApp.git
   cd LibraryApp
   ```
2. Apply SQLite database migrations:
   ```bash
   dotnet ef database update --project LibraryApp.Infrastructure
   ```
3. Run the application:
   ```bash
   dotnet run --project LibraryApp.API
   ```
4. Open the web UI at:
   ```
   https://localhost:7012
   ```
   or
   ```
   http://localhost:5137
   ```
5. Access API documentation via Swagger at:
   ```
   https://localhost:7012/swagger
   ```

---

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/books` | List all books |
| GET | `/api/books/{id}` | Get a book by ID |
| GET | `/api/books/search?q={query}` | Search books by title, author, or ISBN |
| POST | `/api/books` | Add a new book |
| POST | `/api/books/{id}/borrow` | Borrow a book (decreases available copies) |
| POST | `/api/books/{id}/return` | Return a book (increases available copies) |
| GET | `/api/books/{id}/history` | Get borrowing history for a book |

### Example: Add a Book

```bash
POST /api/books
Content-Type: application/json

{
  "title": "C# in Depth",
  "author": "Jon Snow",
  "year": 2021,
  "isbn": "1234567890123",
  "copies": 5
}
```

### Example: Borrow a Book

```bash
POST /api/books/{id}/borrow
Response:
- 204 No Content if successful
- 400 Bad Request if no copies are available
```

### Example: Return a Book

```bash
POST /api/books/{id}/return
Response:
- 204 No Content
```

---

## UI Features

- MVC web interface:
  - View all books
  - Add new books
  - Borrow/return books
  - Search by title, author, or ISBN
  - View borrowing history
- Access at `/` when the API is running

---

## Running Tests

- The project uses xUnit for integration tests.
- Tests use an in-memory SQLite database to avoid conflicts with production data.
- Run all tests:
```bash
dotnet test
```

---

## Behavior Notes

- Borrowing a book with no available copies returns a 400 Bad Request.
- Returning a book that hasn’t been borrowed is idempotent (no error, no state change).
- All endpoints include validation and logging.
- SQLite is the default persistence provider; in-memory database is used for tests.

---

## AI Assistance

- AI assisted in:
  - Code scaffolding and architecture decisions
  - Identifying test opportunities
  - Generating documentation