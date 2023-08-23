using Microsoft.AspNetCore.Mvc;
using Website.Api.Features.Library.Models;
using Microsoft.AspNetCore.Authorization;
using Website.Api.Features.Library.Services;

namespace Website.Api.Features.Library.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class BooksController : ControllerBase
{
    private readonly ILibraryService _library;
    public BooksController(ILibraryService libraryService)
    {
        _library = libraryService;
    }

    [HttpPost]
    public async Task<Book> CreateBook(CreateBookReq req)
    {
        return await _library.CreateBook(req.LibraryId, req.Content);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBook(string id)
    {
        var book = await _library.GetBook(id);

        if (book is null)
            return NotFound();

        return Ok(book);
    }

    [HttpPut("{id}")]
    public async Task UpdateBook(string id, BookContent content)
    {
        await _library.UpdateBook(id, content);
    }

    // [HttpDelete("{id}")]
    // public async Task DeleteBook(string id) => await _library.DeleteBook(id);
}

public record CreateBookReq(string LibraryId, BookContent Content);