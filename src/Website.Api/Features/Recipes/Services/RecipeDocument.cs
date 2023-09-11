using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Website.Api.Features.Recipes.Models;

public class RecipeDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public required string Name { get; set; }

    public string? ImageUrl { get; set; }

    public int PeopleCount { get; set; }

    public TimeComponent Time { get; set; } = new TimeComponent();

    public List<string> Tags { get; set; } = new();

    public List<string> Ingredients { get; set; } = new();

    public List<RecipeStepDocument> Steps { get; set; } = new();
}

