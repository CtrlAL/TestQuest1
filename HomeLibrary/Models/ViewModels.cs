using System.Collections.Immutable;
using HomeLibrary.Services;

namespace HomeLibrary.Models;

public sealed record TocSection
{
    public TocSection(int level, string title, ImmutableArray<TocSection> children)
    {
        if (level is < 1 or > 6)
        {
            throw new ArgumentOutOfRangeException(nameof(level), level, "Уровень раздела должен быть от 1 до 6.");
        }

        ArgumentNullException.ThrowIfNull(title);

        if (children.IsDefault)
        {
            throw new ArgumentNullException(nameof(children));
        }

        Level = level;
        Title = title;
        Children = children;
    }

    public int Level { get; }

    public string Title { get; }

    public ImmutableArray<TocSection> Children { get; }

    public bool Equals(TocSection? other) =>
        other is not null
        && Level == other.Level
        && Title == other.Title
        && Children.SequenceEqual(other.Children);

    public override int GetHashCode() => HashCode.Combine(Level, Title, Children.Length);
}

public sealed record TocDocument
{
    public TocDocument(ImmutableArray<TocSection> sections)
    {
        if (sections.IsDefault)
        {
            throw new ArgumentNullException(nameof(sections));
        }

        Sections = sections;
    }

    public ImmutableArray<TocSection> Sections { get; }

    public bool Equals(TocDocument? other) =>
        other is not null && Sections.SequenceEqual(other.Sections);

    public override int GetHashCode() => Sections.Length;
}

public sealed record BookListViewModel(string Query, IReadOnlyList<Book> Books);

public sealed record BookDetailsViewModel(
    Book Book,
    IReadOnlyList<TocSection> Sections,
    BookFormModel TocForm);
