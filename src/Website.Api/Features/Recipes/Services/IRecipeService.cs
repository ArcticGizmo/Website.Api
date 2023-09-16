using Website.Api.Common;
using Website.Api.Features.Recipes.Models;

namespace Website.Api.Features.Recipes.Services;

public interface IRecipeService
{
    public Task<PagedData<Recipe>> GetRecipes(RecipeQueryOptions opts);

    public Task<Recipe?> GetRecipe(string recipeId);

    public Task<Recipe> CreateRecipe(RecipeContent content);

    public Task UpdateRecipe(string recipeId, RecipeContent content);

    public Task DeleteRecipe(string recipeId);

}
