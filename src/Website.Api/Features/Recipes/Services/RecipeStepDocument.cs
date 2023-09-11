namespace Website.Api.Features.Recipes.Models;

public class RecipeStepDocument
{
    public required string Text { get; set; }

    public required List<string> ImageUrls { get; set; }
}
