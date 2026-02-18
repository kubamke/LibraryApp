namespace LibraryApp.Application.DTOs;

public class BorrowRecordDto
{
    public Guid Id { get; set; }
    public DateTime BorrowedAt { get; set; }
    public DateTime? ReturnedAt { get; set; }
}