using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HomeLibrary.Models;

public sealed class BookFormModel
{
    [BindNever]
    public long Id { get; set; }

    [Required]
    [StringLength(300, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(300, MinimumLength = 1)]
    public string Author { get; set; } = string.Empty;

    [Range(1000, 9999)]
    public int PublicationYear { get; set; } = DateTime.UtcNow.Year;

    [StringLength(20)]
    public string? Isbn { get; set; }

    [StringLength(100)]
    public string? Genre { get; set; }

    [StringLength(200)]
    public string? Publisher { get; set; }

    [Range(1, int.MaxValue)]
    public int? PageCount { get; set; }

    public bool ReadStatus { get; set; }

    [Required]
    public string TocXml { get; set; } = "<toc />";

    [BindNever]
    public string TocEditorHtml { get; set; } = string.Empty;

    public BookWriteModel ToWriteModel() => new(
        Title.Trim(),
        Author.Trim(),
        PublicationYear,
        TocXml,
        Normalize(Isbn),
        Normalize(Genre),
        Normalize(Publisher),
        PageCount,
        ReadStatus);

    public static BookFormModel FromBook(Book book) => new()
    {
        Id = book.Id,
        Title = book.Title,
        Author = book.Author,
        PublicationYear = book.PublicationYear,
        Isbn = book.Isbn,
        Genre = book.Genre,
        Publisher = book.Publisher,
        PageCount = book.PageCount,
        ReadStatus = book.ReadStatus,
        TocXml = book.TocXml
    };

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
