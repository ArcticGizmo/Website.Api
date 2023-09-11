using Website.Api.Features.Recipes.Models;

namespace Website.Api.Features.Recipes.Services;

public interface IRecipeService
{
    public Task<IList<Recipe>> GetRecipes(RecipeQueryOptions? opts = null);

    public Task<Recipe?> GetRecipe(string recipeId);

    public Task<Recipe> CreateRecipe(RecipeContent content);

    public Task UpdateRecipe(string recipeId, RecipeContent content);

    public Task DeleteRecipe(string recipeId);

}
