using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Website.Api.Features.Library.Services;

public class BookDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("libraryId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string LibraryId { get; set; }

    [BsonElement("isbn")]
    public string? Isbn { get; set; }
    [BsonElement("title")]

    public required string Title { get; set; }

    [BsonElement("authors")]

    public required List<string> Authors { get; set; } = new();

    [BsonElement("pageCount")]
    public int? PageCount { get; set; }

    [BsonElement("series")]

    public string? Series { get; set; }

    [BsonElement("bookInSeries")]
    public float? BookInSeries { get; set; }

    [BsonElement("tags")]

    public List<string> Tags { get; set; } = new();

    [BsonElement("rating")]

    public float? Rating { get; set; }

    [BsonElement("coverImageUrl")]
    public string? CoverImageUrl { get; set; }
}

public class BookSeriesDocument
{
    [BsonElement("name")]

    public required string Name { get; set; }

    [BsonElement("bookInSeries")]

    public required int BookInSeries { get; set; }
}