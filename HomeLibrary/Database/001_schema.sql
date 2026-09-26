CREATE TABLE IF NOT EXISTS books
(
    id                bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    title             varchar(300) NOT NULL,
    author            varchar(300) NOT NULL,
    publication_year  integer      NOT NULL,
    toc_xml           xml          NOT NULL DEFAULT '<toc />'::xml,
    isbn              varchar(20),
    genre             varchar(100),
    publisher         varchar(200),
    page_count        integer,
    read_status       boolean      NOT NULL DEFAULT false,
    added_at          timestamptz  NOT NULL DEFAULT now(),
    updated_at        timestamptz  NOT NULL DEFAULT now(),
    search_vector     tsvector GENERATED ALWAYS AS
    (
        setweight(to_tsvector('simple', coalesce(title, '')), 'A') ||
        setweight(to_tsvector('simple', coalesce(author, '')), 'A') ||
        setweight(to_tsvector('simple', coalesce(toc_xml::text, '')), 'B')
    ) STORED,
    CONSTRAINT books_title_not_blank CHECK (btrim(title) <> ''),
    CONSTRAINT books_author_not_blank CHECK (btrim(author) <> ''),
    CONSTRAINT books_year_range CHECK (publication_year BETWEEN 1000 AND 9999),
    CONSTRAINT books_page_count_positive CHECK (page_count IS NULL OR page_count > 0)
);

CREATE INDEX IF NOT EXISTS ix_books_search_vector
    ON books USING gin (search_vector);
