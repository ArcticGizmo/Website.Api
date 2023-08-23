using Website.Api.Features.Library.Models;

namespace Website.Api.Features.Library.Services;

public interface ILibraryService
{
    public Task<IList<LibraryDefinition>> GetLibraries(string userId);

    public Task<LibraryDefinition?> GetLibrary(string libraryId);

    public Task<LibraryDefinition> CreateLibrary(LibraryDefinition library);

    public Task UpdateLibrary(string libraryId, LibraryDefinition library);

    public Task DeleteLibrary(string id);

    public Task<IList<Book>> GetBooks(string libraryId, BookQueryOptions? opts = null);

    public Task<Book?> GetBook(string bookId);

    public Task<Book> CreateBook(string libraryId, BookContent content);

    public Task UpdateBook(string bookId, BookContent content);

    public Task DeleteBook(string bookId);
}