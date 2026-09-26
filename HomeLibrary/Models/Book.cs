namespace HomeLibrary.Models;

public sealed record Book(
    long Id,
    string Title,
    string Author,
    int PublicationYear,
    string TocXml,
    string? Isbn,
    string? Genre,
    string? Publisher,
    int? PageCount,
    bool ReadStatus,
    DateTimeOffset AddedAt,
    DateTimeOffset UpdatedAt);

public sealed record BookWriteModel(
    string Title,
    string Author,
    int PublicationYear,
    string TocXml,
    string? Isbn,
    string? Genre,
    string? Publisher,
    int? PageCount,
    bool ReadStatus);
