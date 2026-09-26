using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Xml;
using System.Xml.Linq;
using HomeLibrary.Models;

namespace HomeLibrary.Services;

public sealed class TocService : ITocService
{
    private static readonly HtmlEncoder TitleEncoder = HtmlEncoder.Create(UnicodeRanges.All);

    private static readonly XmlReaderSettings ReaderSettings = new()
    {
        DtdProcessing = DtdProcessing.Prohibit,
        XmlResolver = null,
        MaxCharactersInDocument = 1_000_000,
    };

    public TocDocument Parse(string? xml)
    {
        if (string.IsNullOrWhiteSpace(xml))
        {
            return new TocDocument([]);
        }

        XDocument document;
        try
        {
            using var text = new StringReader(xml);
            using var reader = XmlReader.Create(text, ReaderSettings);
            document = XDocument.Load(reader, LoadOptions.None);
        }
        catch (XmlException exception)
        {
            throw new TocValidationException("Оглавление содержит некорректный XML.", exception);
        }

        var root = document.Root
            ?? throw new TocValidationException("Оглавление должно иметь корневой элемент toc.");

        if (root.Name != "toc" || root.HasAttributes)
        {
            throw new TocValidationException("Корневой элемент оглавления должен быть toc без атрибутов.");
        }

        if (root.Nodes().Any(node => node switch
            {
                XText text => !string.IsNullOrWhiteSpace(text.Value),
                XElement element => element.Name != "section",
                _ => true
            }))
        {
            throw new TocValidationException("Внутри toc допустимы только элементы section.");
        }

        var sections = root.Elements().Select(element => ParseSection(element, null)).ToImmutableArray();
        return new TocDocument(sections);
    }

    public string ToEditorHtml(TocDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        var builder = new StringBuilder();
        AppendSections(document.Sections, builder);
        return builder.ToString();
    }

    private static TocSection ParseSection(XElement element, int? parentLevel)
    {
        if (element.Name != "section")
        {
            throw new TocValidationException("Внутри toc допустимы только элементы section.");
        }

        if (HasUnexpectedContent(element))
        {
            throw new TocValidationException("Элемент section не должен содержать текст или комментарии.");
        }

        var attributes = element.Attributes().ToArray();
        if (attributes.Length != 1 || attributes[0].Name != "level")
        {
            throw new TocValidationException("Элемент section должен иметь единственный атрибут level.");
        }

        if (!int.TryParse(attributes[0].Value, NumberStyles.None, CultureInfo.InvariantCulture, out var level)
            || level is < 1 or > 6)
        {
            throw new TocValidationException("Уровень section должен быть целым числом от 1 до 6.");
        }

        if (parentLevel.HasValue && level <= parentLevel.Value)
        {
            throw new TocValidationException("Уровень вложенного section должен быть больше родительского.");
        }

        var children = element.Elements().ToArray();
        if (children.Length == 0
            || children.Count(child => child.Name == "title") != 1
            || children[0].Name != "title"
            || children.Skip(1).Any(child => child.Name != "section"))
        {
            throw new TocValidationException("Элемент section должен содержать title, а затем вложенные section.");
        }

        if (children[0].HasAttributes
            || children[0].HasElements
            || children[0].Nodes().Any(node => node is not XText))
        {
            throw new TocValidationException("Элемент title должен содержать только текст.");
        }

        var title = string.Concat(children[0].Nodes().OfType<XText>().Select(node => node.Value)).Trim();
        if (title.Length == 0)
        {
            throw new TocValidationException("Элемент title не должен быть пустым.");
        }

        var nested = children.Skip(1)
            .Select(child => ParseSection(child, level))
            .ToImmutableArray();

        return new TocSection(level, title, nested);
    }

    private static bool HasUnexpectedContent(XElement element) =>
        element.Nodes().Any(node => node switch
        {
            XText text => !string.IsNullOrWhiteSpace(text.Value),
            XElement => false,
            _ => true
        });

    private static void AppendSections(IEnumerable<TocSection> sections, StringBuilder builder)
    {
        foreach (var section in sections)
        {
            builder.Append("<h")
                .Append(section.Level)
                .Append('>')
                .Append(TitleEncoder.Encode(section.Title))
                .Append("</h")
                .Append(section.Level)
                .Append('>');

            AppendSections(section.Children, builder);
        }
    }
}
