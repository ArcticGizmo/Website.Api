using Microsoft.AspNetCore.Mvc;
using Website.Api.Features.Library.Models;

using Microsoft.AspNetCore.Authorization;
using Website.Api.Features.Library.Services;
using Website.Api.Common;

namespace Website.Api.Features.Library.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class LibrariesController : ControllerBase
{
    private readonly ILibraryService _library;
    public LibrariesController(ILibraryService libraryService)
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
    public async Task<IActionResult> GetLibrary(string id)
    {
        var library = await _library.GetLibrary(id);
        if (library == null)
            return NotFound();
        return Ok(library);
    }

    [HttpPut("{id}")]
    public async Task UpdateLibrary(string id, LibraryDefinition def)
    {
        def.Id = id;
        await _library.UpdateLibrary(id, def);
    }

    // [HttpDelete("{id}")]
    // public async Task DeleteLibrary(string id)
    // {
    //     await _library.DeleteLibrary(id);
    // }

    [HttpGet("{libraryId}/books")]
    public async Task<PagedData<Book>> GetBooks(string libraryId, [FromQuery] BookQueryOptions opts)
    {
        return await _library.GetBooks(libraryId, opts);
    }
}

public record CreateLibraryReq(string Name);

public record GetLibraryRes(LibraryDefinition Library, IList<Book> Books);