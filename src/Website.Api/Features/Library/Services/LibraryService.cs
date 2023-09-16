using Website.Api.Common;
using Website.Api.Features.Library.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Website.Api.Features.Library.Services;

public class LibraryService : ILibraryService
{
    const string LibraryCollection = "Libraries";
    const string BookCollection = "Books";

    private static Collation CollationEn = new Collation("en", strength: CollationStrength.Primary);

    private readonly IMongoDatabase _db;
    private readonly IMongoCollection<LibraryDocument> _libraryCollection;
    private readonly IMongoCollection<BookDocument> _booksCollection;

    public LibraryService(IOptions<LibraryDatabaseConfig> libraryDatabaseConfig)
    {
        var client = new MongoClient(libraryDatabaseConfig.Value.ConnectionString);

        _db = client.GetDatabase(libraryDatabaseConfig.Value.Database);

        _libraryCollection = _db.GetCollection<LibraryDocument>(LibraryCollection);
        _booksCollection = _db.GetCollection<BookDocument>(BookCollection);
    }

    public async Task<IList<LibraryDefinition>> GetLibraries(string userId)
    {
        var items = await _libraryCollection.Find(_ => true).ToListAsync();
        return items.Select(i => i.ToLibraryDefinition()).ToList();
    }

    public async Task<LibraryDefinition?> GetLibrary(string libraryId)
    {
        if (!ObjectId.TryParse(libraryId, out _))
            return null;
        var item = await _libraryCollection.Find(x => x.Id == libraryId).FirstOrDefaultAsync();
        return item?.ToLibraryDefinition();
    }

    public async Task UpdateLibrary(string libraryId, LibraryDefinition library)
    {
        // is this atomic? No, but it sure is easier
        var doc = await _libraryCollection.Find(x => x.Id == libraryId).FirstAsync();

        doc.Name = library.Name;

        await _libraryCollection.ReplaceOneAsync(x => x.Id == libraryId, doc);
    }


    public async Task<LibraryDefinition> CreateLibrary(LibraryDefinition library)
    {
        var doc = new LibraryDocument { Name = library.Name, OwnerUserId = library.OwnerUserId };
        await _libraryCollection.InsertOneAsync(doc);
        return doc.ToLibraryDefinition();
    }

    public async Task DeleteLibrary(string libraryId)
    {
        // TODO: this is not attomic
        await _booksCollection.DeleteManyAsync(x => x.LibraryId == libraryId);
        await _libraryCollection.DeleteOneAsync(x => x.Id == libraryId);
    }

    public async Task<PagedData<Book>> GetBooks(string libraryId, BookQueryOptions opts)
    {
        ObjectId id;
        if (!ObjectId.TryParse(libraryId, out id))
            return new PagedData<Book>(new List<Book>(), 0, 0, null);

        IFindFluent<BookDocument, BookDocument> query;

        if (string.IsNullOrWhiteSpace(opts.SearchText))
        {
            query = _booksCollection.Find(x => x.LibraryId == libraryId, new FindOptions { Collation = CollationEn });
        }
        else
        {
            var filter = GetBookTextFilter(id, opts.SearchText);
            query = _booksCollection.Find(filter, new FindOptions { Collation = CollationEn });
        }


        var rawItems = await query.SortBy(opts).Paged(opts).ToListAsync();
        var items = rawItems.Select(d => d.ToBook()).ToList();

        int? nextPage = items.Count < opts.PageSize ? null : opts.PageNumber + 1;
        return new PagedData<Book>(items, opts.PageNumber, opts.PageSize, nextPage);
    }

    private FilterDefinition<BookDocument> GetBookTextFilter(ObjectId libraryId, string searchText)
    {
        var reg = new BsonRegularExpression(searchText, "i");
        var builder = Builders<BookDocument>.Filter;
        return builder.Eq("libraryId", libraryId) & (builder.Regex("title", reg) | builder.Regex("isbn", searchText) | builder.Regex("series", reg) | builder.AnyEq("authors", reg));
    }

    public async Task<Book?> GetBook(string bookId)
    {
        if (!ObjectId.TryParse(bookId, out _))
            return null;
        var doc = await _booksCollection.Find(x => x.Id == bookId).FirstOrDefaultAsync();
        return doc?.ToBook();
    }

    public async Task<Book> CreateBook(string libraryId, BookContent content)
    {
        var doc = new BookDocument
        {
            LibraryId = libraryId,
            Isbn = content.Isbn,
            Title = content.Title,
            Authors = content.Authors,
            Series = content.Series,
            BookInSeries = content.BookInSeries,
            Tags = content.Tags,
            Rating = content.Rating,
            CoverImageUrl = content.CoverImageUrl,
            PageCount = content.PageCount,
            Notes = content.Notes,
            Read = content.Read,
            Wishlist = content.Wishlist,
        };
        await _booksCollection.InsertOneAsync(doc);
        return doc.ToBook();
    }

    public async Task UpdateBook(string bookId, BookContent content)
    {
        // is this atomic? No, but it sure is easier
        var doc = await _booksCollection.Find(x => x.Id == bookId).FirstAsync();
        doc.Isbn = content.Isbn;
        doc.Title = content.Title;
        doc.Authors = content.Authors;
        doc.Series = content.Series;
        doc.BookInSeries = content.BookInSeries;
        doc.Tags = content.Tags;
        doc.Rating = content.Rating;
        doc.CoverImageUrl = content.CoverImageUrl;
        doc.PageCount = content.PageCount;
        doc.Notes = content.Notes;
        doc.Read = content.Read;
        doc.Wishlist = content.Wishlist;

        await _booksCollection.ReplaceOneAsync(x => x.Id == bookId, doc);
    }

    public async Task DeleteBook(string bookId) => await _booksCollection.DeleteOneAsync(x => x.Id == bookId);

    public async Task<bool> LibraryContainsBook(string libraryId, string isbn)
    {
        return await _booksCollection.Find(x => x.LibraryId == libraryId && x.Isbn == isbn).AnyAsync();
    }

}

internal static class Extensions
{
    public static LibraryDefinition ToLibraryDefinition(this LibraryDocument doc)
    {
        var id = string.IsNullOrWhiteSpace(doc.Id) ? null : doc.Id;
        return new LibraryDefinition()
        {
            Id = id,
            Name = doc.Name,
            OwnerUserId = doc.OwnerUserId,
        };
    }

    public static Book ToBook(this BookDocument doc)
    {
        return new Book()
        {
            Id = doc.Id!,
            LibraryId = doc.LibraryId,
            Content = new()
            {
                Title = doc.Title,
                Isbn = doc.Isbn,
                Authors = doc.Authors,
                BookInSeries = doc.BookInSeries,
                Series = doc.Series,
                Tags = doc.Tags,
                Rating = doc.Rating,
                CoverImageUrl = doc.CoverImageUrl,
                PageCount = doc.PageCount,
                Notes = doc.Notes,
                Read = doc.Read,
                Wishlist = doc.Wishlist,
            }
        };
    }
}

