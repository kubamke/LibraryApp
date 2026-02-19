namespace LibraryApp.Application.DTOs;

using System.ComponentModel.DataAnnotations;

public class CreateBookDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Author { get; set; } = string.Empty;

    [Range(1500, int.MaxValue)] 
    public int Year { get; set; }

    [Required]
    [RegularExpression(@"^\d{10}(\d{3})?$",
        ErrorMessage = "ISBN must be 10 or 13 digits.")]
    public string ISBN { get; set; } = string.Empty;

    [Range(0, 10000)]
    public int Copies { get; set; }
}
