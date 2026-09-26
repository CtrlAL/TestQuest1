using System.Data;
using HomeLibrary.Models;
using Npgsql;
using NpgsqlTypes;

namespace HomeLibrary.Data;

public sealed class NpgsqlBookRepository(NpgsqlDataSource dataSource) : IBookRepository
{
    private static readonly string[] BookParameterNames =
    [
        "p_title",
        "p_author",
        "p_publication_year",
        "p_toc_xml",
        "p_isbn",
        "p_genre",
        "p_publisher",
        "p_page_count",
        "p_read_status"
    ];

    public async Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await ReadManyAsync(Call("usp_books_list"), [], cancellationToken);

    public async Task<IReadOnlyList<Book>> SearchAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        var parameters = new[] { Text("p_query", query) };
        return await ReadManyAsync(Call("usp_books_search", "p_query"), parameters, cancellationToken);
    }

    public async Task<Book?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var parameters = new[] { BigInt("p_book_id", id) };
        return (await ReadManyAsync(Call("usp_books_get", "p_book_id"), parameters, cancellationToken))
            .FirstOrDefault();
    }

    public async Task<long> CreateAsync(
        BookWriteModel book,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(
            Call("usp_books_insert", BookParameterNames),
            connection)
        {
            CommandType = CommandType.Text
        };

        AddBookParameters(command, book);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            throw new InvalidOperationException("usp_books_insert returned no rows.");
        }

        return reader.GetInt64(0);
    }

    public async Task<bool> UpdateAsync(
        long id,
        BookWriteModel book,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(
            Call("usp_books_update", ["p_id", .. BookParameterNames]),
            connection)
        {
            CommandType = CommandType.Text
        };

        command.Parameters.Add(BigInt("p_id", id));
        AddBookParameters(command, book);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            throw new InvalidOperationException("usp_books_update returned no rows.");
        }

        return reader.GetBoolean(0);
    }

    public async Task<bool> UpdateTocAsync(
        long id,
        string tocXml,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(
            Call("usp_books_update_toc", "p_id", "p_toc_xml"),
            connection)
        {
            CommandType = CommandType.Text
        };

        command.Parameters.Add(BigInt("p_id", id));
        command.Parameters.Add(Xml("p_toc_xml", tocXml));

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            throw new InvalidOperationException("usp_books_update_toc returned no rows.");
        }

        return reader.GetBoolean(0);
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(Call("usp_books_delete", "p_id"), connection)
        {
            CommandType = CommandType.Text
        };

        command.Parameters.Add(BigInt("p_id", id));

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            throw new InvalidOperationException("usp_books_delete returned no rows.");
        }

        return reader.GetBoolean(0);
    }

    private async Task<IReadOnlyList<Book>> ReadManyAsync(
        string call,
        IReadOnlyCollection<NpgsqlParameter> parameters,
        CancellationToken cancellationToken)
    {
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(call, connection)
        {
            CommandType = CommandType.Text
        };

        command.Parameters.AddRange(parameters.ToArray());

        var books = new List<Book>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            books.Add(ReadBook(reader));
        }

        return books;
    }

    private static void AddBookParameters(NpgsqlCommand command, BookWriteModel book)
    {
        command.Parameters.Add(Text("p_title", book.Title));
        command.Parameters.Add(Text("p_author", book.Author));
        command.Parameters.Add(Integer("p_publication_year", book.PublicationYear));
        command.Parameters.Add(Xml("p_toc_xml", book.TocXml));
        command.Parameters.Add(Text("p_isbn", book.Isbn));
        command.Parameters.Add(Text("p_genre", book.Genre));
        command.Parameters.Add(Text("p_publisher", book.Publisher));
        command.Parameters.Add(Integer("p_page_count", book.PageCount));
        command.Parameters.Add(Boolean("p_read_status", book.ReadStatus));
    }

    private static Book ReadBook(NpgsqlDataReader reader) => new(
        reader.GetInt64(Ordinal(reader, "id")),
        reader.GetString(Ordinal(reader, "title")),
        reader.GetString(Ordinal(reader, "author")),
        reader.GetInt32(Ordinal(reader, "publication_year")),
        reader.GetString(Ordinal(reader, "toc_xml")),
        NullableString(reader, "isbn"),
        NullableString(reader, "genre"),
        NullableString(reader, "publisher"),
        NullableInt32(reader, "page_count"),
        reader.GetBoolean(Ordinal(reader, "read_status")),
        reader.GetFieldValue<DateTimeOffset>(Ordinal(reader, "added_at")),
        reader.GetFieldValue<DateTimeOffset>(Ordinal(reader, "updated_at")));

    private static int Ordinal(NpgsqlDataReader reader, string name) => reader.GetOrdinal(name);

    private static string? NullableString(NpgsqlDataReader reader, string name) =>
        reader.IsDBNull(Ordinal(reader, name)) ? null : reader.GetString(Ordinal(reader, name));

    private static int? NullableInt32(NpgsqlDataReader reader, string name) =>
        reader.IsDBNull(Ordinal(reader, name)) ? null : reader.GetInt32(Ordinal(reader, name));

    // Хранимые объекты - функции с RETURNS TABLE, поэтому они вызываются
    // в SQL-форме, а не через CommandType.StoredProcedure: процедура в
    // PostgreSQL не умеет возвращать набор строк.
    private static string Call(string functionName, params string[] parameterNames) =>
        $"SELECT * FROM {functionName}({string.Join(", ", parameterNames.Select(name => "@" + name))})";

    private static NpgsqlParameter Text(string name, string? value) =>
        new(name, NpgsqlDbType.Text) { Value = (object?)value ?? DBNull.Value };

    private static NpgsqlParameter BigInt(string name, long value) =>
        new(name, NpgsqlDbType.Bigint) { Value = value };

    private static NpgsqlParameter Integer(string name, int? value) =>
        new(name, NpgsqlDbType.Integer) { Value = (object?)value ?? DBNull.Value };

    private static NpgsqlParameter Boolean(string name, bool value) =>
        new(name, NpgsqlDbType.Boolean) { Value = value };

    private static NpgsqlParameter Xml(string name, string? value) =>
        new(name, NpgsqlDbType.Xml) { Value = (object?)value ?? DBNull.Value };
}
