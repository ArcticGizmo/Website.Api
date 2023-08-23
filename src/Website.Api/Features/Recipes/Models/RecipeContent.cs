namespace Website.Api.Features.Recipes.Models;

public class RecipeContent
{
    public required string Name { get; set; }

    public string? ImageUrl { get; set; }
    public List<string> Tags { get; set; } = new();

    public List<string> Ingredients { get; set; } = new();

    public List<RecipeStep> Steps { get; set; } = new();
}

