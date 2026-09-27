using HomeLibrary.Models;
using Microsoft.Extensions.Logging;

namespace HomeLibrary.Data;

/// <summary>
/// Наполняет пустую базу демонстрационными книгами. Работает только через
/// <see cref="IBookRepository"/>, поэтому не добавляет новых обращений к БД
/// и не нарушает правило "репозиторий вызывает только семь usp_books_".
/// </summary>
public sealed class DatabaseSeeder(IBookRepository repository, ILogger<DatabaseSeeder> logger)
{
    /// <summary>
    /// Вставляет <see cref="SeedBooks"/>, только если таблица books пуста.
    /// Повторный запуск не создаёт дубликатов.
    /// </summary>
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await repository.GetAllAsync(cancellationToken) is { Count: > 0 })
        {
            logger.LogInformation("Books table is not empty, skipping demo data.");
            return;
        }

        foreach (var book in SeedBooks.All)
        {
            await repository.CreateAsync(book, cancellationToken);
        }

        logger.LogInformation("Seeded {Count} demo books.", SeedBooks.All.Count);
    }
}
