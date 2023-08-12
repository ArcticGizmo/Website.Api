using Microsoft.AspNetCore.Mvc;
using Website.Api.Features.Library.Models;

using Microsoft.AspNetCore.Authorization;
using Website.Api.Features.Library.Services;

namespace Website.Api.Features.Library.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class LibraryController : ControllerBase
{
    private readonly ILibraryService _library;
    public LibraryController(ILibraryService libraryService)
    {
        _library = libraryService;
    }

    private string GetUserId() => User.Identity!.Name!;

    [HttpGet]
    public async Task<IList<LibraryDefinition>> All()
    {
        var userId = GetUserId();
        return await _library.GetLibraries(userId);
    }

    [HttpPost]
    public async Task<LibraryDefinition> CreateLibrary(CreateLibraryReq req)
    {
        var library = new LibraryDefinition()
        {
            Name = req.Name,
            OwnerUserId = GetUserId()
        };
        await _library.CreateLibrary(library);

        return library;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLibrary(string id, [FromQuery] string? searchText = null)
    {
        var library = await _library.GetLibrary(id);
        if (library == null)
            return NotFound();

        var books = await _library.GetBooks(id, searchText);

        return Ok(new GetLibraryRes(library, books));
    }

    [HttpPut("{id}")]
    public async Task UpdateLibrary(string id, LibraryDefinition def)
    {
        def.Id = id;
        await _library.UpdateLibrary(id, def);
    }

    [HttpDelete("{id}")]
    public async Task DeleteLibrary(string id)
    {
        await _library.DeleteLibrary(id);
    }
}

public record CreateLibraryReq(string Name);

public record GetLibraryRes(LibraryDefinition Library, IList<Book> Books);