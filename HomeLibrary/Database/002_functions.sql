CREATE OR REPLACE FUNCTION usp_books_list()
RETURNS TABLE(
    id               bigint,
    title            varchar(300),
    author           varchar(300),
    publication_year integer,
    toc_xml          text,
    isbn             varchar(20),
    genre            varchar(100),
    publisher        varchar(200),
    page_count       integer,
    read_status      boolean,
    added_at         timestamptz,
    updated_at       timestamptz,
    rank             real)
LANGUAGE plpgsql
STABLE
AS $$
BEGIN
    RETURN QUERY
    SELECT b.id,
           b.title,
           b.author,
           b.publication_year,
           b.toc_xml::text,
           b.isbn,
           b.genre,
           b.publisher,
           b.page_count,
           b.read_status,
           b.added_at,
           b.updated_at,
           NULL::real AS rank
    FROM books AS b
    ORDER BY b.title, b.publication_year, b.id;
END;
$$;

CREATE OR REPLACE FUNCTION usp_books_get(p_book_id bigint)
RETURNS TABLE(
    id               bigint,
    title            varchar(300),
    author           varchar(300),
    publication_year integer,
    toc_xml          text,
    isbn             varchar(20),
    genre            varchar(100),
    publisher        varchar(200),
    page_count       integer,
    read_status      boolean,
    added_at         timestamptz,
    updated_at       timestamptz,
    rank             real)
LANGUAGE plpgsql
STABLE
AS $$
BEGIN
    RETURN QUERY
    SELECT b.id,
           b.title,
           b.author,
           b.publication_year,
           b.toc_xml::text,
           b.isbn,
           b.genre,
           b.publisher,
           b.page_count,
           b.read_status,
           b.added_at,
           b.updated_at,
           NULL::real AS rank
    FROM books AS b
    WHERE b.id = p_book_id;
END;
$$;

CREATE OR REPLACE FUNCTION usp_books_search(p_query text)
RETURNS TABLE(
    id               bigint,
    title            varchar(300),
    author           varchar(300),
    publication_year integer,
    toc_xml          text,
    isbn             varchar(20),
    genre            varchar(100),
    publisher        varchar(200),
    page_count       integer,
    read_status      boolean,
    added_at         timestamptz,
    updated_at       timestamptz,
    rank             real)
LANGUAGE plpgsql
STABLE
AS $$
DECLARE
    v_query   text;
    v_pattern text;
    v_tsquery tsquery;
BEGIN
    v_query := btrim(coalesce(p_query, ''));

    IF v_query = '' THEN
        RETURN QUERY
        SELECT b.id,
               b.title,
               b.author,
               b.publication_year,
               b.toc_xml::text,
               b.isbn,
               b.genre,
               b.publisher,
               b.page_count,
               b.read_status,
               b.added_at,
               b.updated_at,
               NULL::real AS rank
        FROM books AS b
        ORDER BY b.title, b.publication_year, b.id;
        RETURN;
    END IF;

    v_pattern := replace(replace(replace(v_query, '\', '\\'), '%', '\%'), '_', '\_');
    v_tsquery := websearch_to_tsquery('simple', v_query);

    RETURN QUERY
    SELECT b.id,
           b.title,
           b.author,
           b.publication_year,
           b.toc_xml::text,
           b.isbn,
           b.genre,
           b.publisher,
           b.page_count,
           b.read_status,
           b.added_at,
           b.updated_at,
           ts_rank(b.search_vector, v_tsquery) AS rank
    FROM books AS b
    WHERE b.search_vector @@ v_tsquery
       OR b.title ILIKE '%' || v_pattern || '%' ESCAPE '\'
       OR b.author ILIKE '%' || v_pattern || '%' ESCAPE '\'
       OR b.toc_xml::text ILIKE '%' || v_pattern || '%' ESCAPE '\'
    ORDER BY ts_rank(b.search_vector, v_tsquery) DESC, b.title, b.publication_year, b.id;
END;
$$;

CREATE OR REPLACE FUNCTION usp_books_insert(
    p_title text,
    p_author text,
    p_publication_year integer,
    p_toc_xml xml,
    p_isbn text,
    p_genre text,
    p_publisher text,
    p_page_count integer,
    p_read_status boolean)
RETURNS TABLE(id bigint)
LANGUAGE plpgsql
AS $$
DECLARE
    v_id bigint;
BEGIN
    INSERT INTO books AS b
    (
        title,
        author,
        publication_year,
        toc_xml,
        isbn,
        genre,
        publisher,
        page_count,
        read_status,
        added_at,
        updated_at
    )
    VALUES
    (
        btrim(p_title),
        btrim(p_author),
        p_publication_year,
        coalesce(p_toc_xml, '<toc />'::xml),
        nullif(btrim(p_isbn), ''),
        nullif(btrim(p_genre), ''),
        nullif(btrim(p_publisher), ''),
        p_page_count,
        p_read_status,
        now(),
        now()
    )
    RETURNING b.id INTO v_id;

    RETURN QUERY SELECT v_id AS id;
END;
$$;

CREATE OR REPLACE FUNCTION usp_books_update(
    p_id bigint,
    p_title text,
    p_author text,
    p_publication_year integer,
    p_toc_xml xml,
    p_isbn text,
    p_genre text,
    p_publisher text,
    p_page_count integer,
    p_read_status boolean)
RETURNS TABLE(updated boolean)
LANGUAGE plpgsql
AS $$
DECLARE
    v_count integer;
BEGIN
    UPDATE books AS b
    SET title = btrim(p_title),
        author = btrim(p_author),
        publication_year = p_publication_year,
        toc_xml = coalesce(p_toc_xml, b.toc_xml),
        isbn = nullif(btrim(p_isbn), ''),
        genre = nullif(btrim(p_genre), ''),
        publisher = nullif(btrim(p_publisher), ''),
        page_count = p_page_count,
        read_status = p_read_status,
        updated_at = now()
    WHERE b.id = p_id;

    GET DIAGNOSTICS v_count = ROW_COUNT;
    RETURN QUERY SELECT v_count > 0 AS updated;
END;
$$;

CREATE OR REPLACE FUNCTION usp_books_update_toc(
    p_id bigint,
    p_toc_xml xml)
RETURNS TABLE(updated boolean)
LANGUAGE plpgsql
AS $$
DECLARE
    v_count integer;
BEGIN
    UPDATE books AS b
    SET toc_xml = coalesce(p_toc_xml, '<toc />'::xml),
        updated_at = now()
    WHERE b.id = p_id;

    GET DIAGNOSTICS v_count = ROW_COUNT;
    RETURN QUERY SELECT v_count > 0 AS updated;
END;
$$;

CREATE OR REPLACE FUNCTION usp_books_delete(
    p_id bigint)
RETURNS TABLE(deleted boolean)
LANGUAGE plpgsql
AS $$
DECLARE
    v_count integer;
BEGIN
    DELETE FROM books AS b
    WHERE b.id = p_id;

    GET DIAGNOSTICS v_count = ROW_COUNT;
    RETURN QUERY SELECT v_count > 0 AS deleted;
END;
$$;
