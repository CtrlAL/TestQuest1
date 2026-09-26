using HomeLibrary.Models;

namespace HomeLibrary.Data;

public interface IBookRepository
{
    Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Book>> SearchAsync(string query, CancellationToken cancellationToken = default);

    Task<Book?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<long> CreateAsync(BookWriteModel book, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(long id, BookWriteModel book, CancellationToken cancellationToken = default);

    Task<bool> UpdateTocAsync(long id, string tocXml, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
