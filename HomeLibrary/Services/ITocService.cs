using HomeLibrary.Models;

namespace HomeLibrary.Services;

public interface ITocService
{
    TocDocument Parse(string? xml);

    string ToEditorHtml(TocDocument document);
}
