using HomeLibrary.Data;
using HomeLibrary.Models;
using HomeLibrary.Services;
using Microsoft.AspNetCore.Mvc;

namespace HomeLibrary.Controllers;

[Route("Books")]
public sealed class BooksController(IBookRepository repository, ITocService tocService) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(string? q, CancellationToken cancellationToken = default)
    {
        var query = q?.Trim() ?? string.Empty;
        var books = query.Length == 0
            ? await repository.GetAllAsync(cancellationToken)
            : await repository.SearchAsync(query, cancellationToken);

        return View(new BookListViewModel(query, books));
    }

    [HttpGet("Create")]
    public IActionResult Create()
    {
        var model = new BookFormModel();
        TryPrepareForm(model);
        return View("BookForm", model);
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookFormModel model, CancellationToken cancellationToken = default)
    {
        if (!TryPrepareForm(model) || !ModelState.IsValid)
        {
            return View("BookForm", model);
        }

        var id = await repository.CreateAsync(model.ToWriteModel(), cancellationToken);
        TempData["StatusMessage"] = "Книга добавлена.";
        return RedirectToAction("Details", new { id });
    }

    [HttpGet("Details/{id:long}")]
    public async Task<IActionResult> Details(long id, CancellationToken cancellationToken = default)
    {
        var book = await repository.GetByIdAsync(id, cancellationToken);
        return book is null ? NotFound() : View("Details", BuildDetails(book));
    }

    [HttpGet("Edit/{id:long}")]
    public async Task<IActionResult> Edit(long id, CancellationToken cancellationToken = default)
    {
        var book = await repository.GetByIdAsync(id, cancellationToken);
        if (book is null)
        {
            return NotFound();
        }

        var model = BookFormModel.FromBook(book);
        TryPrepareForm(model);
        return View("BookForm", model);
    }

    [HttpPost("Edit/{id:long}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long id, BookFormModel model, CancellationToken cancellationToken = default)
    {
        if (!TryPrepareForm(model) || !ModelState.IsValid)
        {
            return View("BookForm", model);
        }

        if (!await repository.UpdateAsync(id, model.ToWriteModel(), cancellationToken))
        {
            return NotFound();
        }

        TempData["StatusMessage"] = "Книга обновлена.";
        return RedirectToAction("Details", new { id });
    }

    [HttpPost("EditToc/{id:long}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditToc(long id, string tocXml, CancellationToken cancellationToken = default)
    {
        var book = await repository.GetByIdAsync(id, cancellationToken);
        if (book is null)
        {
            return NotFound();
        }

        try
        {
            tocService.Parse(tocXml);
        }
        catch (TocValidationException exception)
        {
            ModelState.AddModelError(nameof(BookFormModel.TocXml), exception.Message);
            return View("Details", BuildDetails(book));
        }

        if (!await repository.UpdateTocAsync(id, tocXml, cancellationToken))
        {
            return NotFound();
        }

        TempData["StatusMessage"] = "Оглавление обновлено.";
        return RedirectToAction("Details", new { id });
    }

    [HttpPost("Delete/{id:long}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken = default)
    {
        if (!await repository.DeleteAsync(id, cancellationToken))
        {
            return NotFound();
        }

        TempData["StatusMessage"] = "Книга удалена.";
        return RedirectToAction("Index");
    }

    private BookDetailsViewModel BuildDetails(Book book)
    {
        var document = tocService.Parse(book.TocXml);
        var form = BookFormModel.FromBook(book);
        form.TocEditorHtml = tocService.ToEditorHtml(document);
        return new BookDetailsViewModel(book, document.Sections, form);
    }

    private bool TryPrepareForm(BookFormModel model)
    {
        try
        {
            model.TocEditorHtml = tocService.ToEditorHtml(tocService.Parse(model.TocXml));
            return true;
        }
        catch (TocValidationException exception)
        {
            ModelState.AddModelError(nameof(BookFormModel.TocXml), exception.Message);
            return false;
        }
    }
}
