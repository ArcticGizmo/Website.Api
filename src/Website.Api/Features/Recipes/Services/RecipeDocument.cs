using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Website.Api.Features.Recipes.Models;

public class RecipeDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("Name")]

    public required string Name { get; set; }

    [BsonElement("ImageUrl")]

    public string? ImageUrl { get; set; }

    [BsonElement("PeopleCount")]

    public int PeopleCount { get; set; }

    [BsonElement("Time")]
    public TimeComponent Time { get; set; } = new TimeComponent();

    [BsonElement("Tags")]
    public required List<string> Tags { get; set; } = new();

    [BsonElement("Ingredients")]
    public List<string> Ingredients { get; set; } = new();

    [BsonElement("Steps")]
    public List<RecipeStepDocument> Steps { get; set; } = new();
}

