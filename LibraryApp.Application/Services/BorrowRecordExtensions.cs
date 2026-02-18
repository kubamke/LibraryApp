

namespace LibraryApp.Application.Services
{
    using LibraryApp.Application.DTOs;
    using LibraryApp.Domain.Entities;
    using System.Linq;
    public static class BorrowRecordExtensions
    {
        public static BorrowRecordDto ToDto(this BorrowRecord record)
        {
            return new BorrowRecordDto
            {
                Id = record.Id,
                BorrowedAt = record.BorrowedAt,
                ReturnedAt = record.ReturnedAt
            };
        }

        public static List<BorrowRecordDto> ToDtoList(this IEnumerable<BorrowRecord> records)
        {
            return [.. records.Select(r => r.ToDto())];
        }
    }
}