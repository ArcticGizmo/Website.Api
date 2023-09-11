namespace Website.Api.Features.Recipes.Models;

public class Recipe
{
    public required string Id { get; set; }
    public required RecipeContent Content { get; set; }
}