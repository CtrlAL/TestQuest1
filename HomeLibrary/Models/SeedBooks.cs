namespace HomeLibrary.Models;

/// <summary>
/// Демонстрационные книги для локальной разработки. Используются только
/// загрузчиком <c>DatabaseSeeder</c> и никогда не попадают в тесты.
/// </summary>
public static class SeedBooks
{
    public static IReadOnlyList<BookWriteModel> All { get; } =
    [
        new(
            Title: "Мастер и Маргарита",
            Author: "Михаил Булгаков",
            PublicationYear: 1967,
            TocXml: """
                <toc>
                  <section level="1">
                    <title>Часть первая</title>
                    <section level="2">
                      <title>Глава 1. Писатель</title>
                    </section>
                    <section level="2">
                      <title>Глава 2. Маг</title>
                    </section>
                  </section>
                  <section level="1">
                    <title>Часть вторая</title>
                    <section level="2">
                      <title>Глава 19. Маргарита</title>
                    </section>
                    <section level="2">
                      <title>Глава 24. Мастер дописывает роман</title>
                      <section level="3">
                        <title>Сон на помостах</title>
                      </section>
                    </section>
                  </section>
                  <section level="1">
                    <title>Часть третья</title>
                    <section level="2">
                      <title>Глава 30. Пора прощаться</title>
                    </section>
                  </section>
                </toc>
                """,
            Isbn: "978-5-17-090388-9",
            Genre: "Фантастика",
            Publisher: "Булгаковская энциклопедия",
            PageCount: 480,
            ReadStatus: true),

        new(
            Title: "Преступление и наказание",
            Author: "Фёдор Достоевский",
            PublicationYear: 1866,
            TocXml: """
                <toc>
                  <section level="1">
                    <title>Часть первая</title>
                    <section level="2">
                      <title>I. Разговор с кузнецом</title>
                    </section>
                    <section level="2">
                      <title>II. В темноте</title>
                    </section>
                  </section>
                  <section level="1">
                    <title>Часть вторая</title>
                    <section level="2">
                      <title>III. В грязном месте</title>
                    </section>
                  </section>
                </toc>
                """,
            Isbn: "978-5-17-019165-6",
            Genre: "Классика",
            Publisher: "Азбука",
            PageCount: 671,
            ReadStatus: true),

        new(
            Title: "Война и мир",
            Author: "Лев Толстой",
            PublicationYear: 1869,
            TocXml: """
                <toc>
                  <section level="1">
                    <title>Том первый</title>
                    <section level="2">
                      <title>Часть первая</title>
                    </section>
                  </section>
                  <section level="1">
                    <title>Том четвёртый</title>
                    <section level="2">
                      <title>Эпилог</title>
                    </section>
                  </section>
                </toc>
                """,
            Isbn: "978-5-17-029906-7",
            Genre: "Исторический роман",
            Publisher: "Азбука",
            PageCount: null,
            ReadStatus: false),

        new(
            Title: "Пикник на обочине",
            Author: "Аркадий и Борис Стругацкие",
            PublicationYear: 1972,
            TocXml: """
                <toc>
                  <section level="1">
                    <title>Оглавление</title>
                    <section level="2">
                      <title>Кислород</title>
                    </section>
                    <section level="2">
                      <title>Фестиваль</title>
                    </section>
                    <section level="2">
                      <title>Трусик</title>
                    </section>
                  </section>
                </toc>
                """,
            Isbn: "978-5-17-019372-5",
            Genre: "Фантастика",
            Publisher: "Азбука",
            PageCount: 256,
            ReadStatus: false),

        new(
            Title: "Трудно быть богом",
            Author: "Аркадий и Борис Стругацкие",
            PublicationYear: 1964,
            TocXml: """
                <toc>
                  <section level="1">
                    <title>Глава первая</title>
                  </section>
                  <section level="1">
                    <title>Глава седьмая</title>
                  </section>
                </toc>
                """,
            Isbn: null,
            Genre: "Фантастика",
            Publisher: "Советский писатель",
            PageCount: 224,
            ReadStatus: true),

        new(
            Title: "Мы",
            Author: "Евгений Замятин",
            PublicationYear: 1921,
            TocXml: """
                <toc>
                  <section level="1">
                    <title>Номер один</title>
                    <section level="2">
                      <title>Новый Адам</title>
                    </section>
                  </section>
                  <section level="1">
                    <title>Номер сорок второй</title>
                    <section level="2">
                      <title>Восстание машин</title>
                    </section>
                  </section>
                </toc>
                """,
            Isbn: "978-5-17-084787-4",
            Genre: "Фантастика",
            Publisher: "Азбука",
            PageCount: 320,
            ReadStatus: true),

        new(
            Title: "Дети Арбата",
            Author: "Анатолий Рыбаков",
            PublicationYear: 1971,
            TocXml: """
                <toc>
                  <section level="1">
                    <title>Московский роман</title>
                    <section level="2">
                      <title>Детство</title>
                    </section>
                    <section level="2">
                      <title>Первый залп</title>
                    </section>
                    <section level="2">
                      <title>Гусарек</title>
                    </section>
                  </section>
                </toc>
                """,
            Isbn: "978-5-17-044253-6",
            Genre: "Исторический роман",
            Publisher: "Речь",
            PageCount: 432,
            ReadStatus: false),

        new(
            Title: "Собачье сердце",
            Author: "Михаил Булгаков",
            PublicationYear: 1926,
            TocXml: """
                <toc>
                  <section level="1">
                    <title>Часть первая</title>
                    <section level="2">
                      <title>Борменталь</title>
                    </section>
                  </section>
                  <section level="1">
                    <title>Часть вторая</title>
                    <section level="2">
                      <title>Полиграф</title>
                    </section>
                    <section level="2">
                      <title>Пересадка</title>
                    </section>
                  </section>
                </toc>
                """,
            Isbn: "978-5-17-089033-6",
            Genre: "Фантастика",
            Publisher: "Азбука",
            PageCount: 208,
            ReadStatus: true),

        new(
            Title: "Анна Каренина",
            Author: "Лев Толстой",
            PublicationYear: 1878,
            TocXml: """
                <toc>
                  <section level="1">
                    <title>Часть первая</title>
                    <section level="2">
                      <title>I</title>
                    </section>
                    <section level="2">
                      <title>II</title>
                    </section>
                  </section>
                  <section level="1">
                    <title>Часть восьмая</title>
                    <section level="2">
                      <title>После бала</title>
                    </section>
                  </section>
                </toc>
                """,
            Isbn: "978-5-17-019207-3",
            Genre: "Классика",
            Publisher: "Азбука",
            PageCount: 1220,
            ReadStatus: false),

        new(
            Title: "Хождение по мукам",
            Author: "Алексей Толстой",
            PublicationYear: 1936,
            TocXml: """
                <toc>
                  <section level="1">
                    <title>Детство Никиты</title>
                    <section level="2">
                      <title>Глава первая</title>
                    </section>
                    <section level="2">
                      <title>Глава десятая</title>
                    </section>
                  </section>
                  <section level="1">
                    <title>Отрочество</title>
                  </section>
                </toc>
                """,
            Isbn: "978-5-17-068152-9",
            Genre: "Классика",
            Publisher: "Азбука",
            PageCount: 512,
            ReadStatus: false),

        new(
            Title: "Понедельник начинается в субботу",
            Author: "Аркадий Стругацкий",
            PublicationYear: 1964,
            TocXml: """
                <toc>
                  <section level="1">
                    <title>Предисловие</title>
                  </section>
                  <section level="1">
                    <title>Смерть от рака</title>
                    <section level="2">
                      <title>Вступление</title>
                    </section>
                  </section>
                </toc>
                """,
            Isbn: "978-5-17-011470-6",
            Genre: "Фантастика",
            Publisher: "Речь",
            PageCount: 288,
            ReadStatus: true),

        // Пустое оглавление и незаполненные необязательные поля: проверка
        // граничных случаев отображения (toc_xml NOT NULL, остальное NULL).
        new(
            Title: "Записки из подполья",
            Author: "Фёдор Достоевский",
            PublicationYear: 1864,
            TocXml: "<toc />",
            Isbn: null,
            Genre: null,
            Publisher: null,
            PageCount: null,
            ReadStatus: false)
    ];
}
