using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Website.Api.Features.Library.Models;

public class LibraryDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public required string Name { get; set; }

    public required string OwnerUserId { get; set; }
}