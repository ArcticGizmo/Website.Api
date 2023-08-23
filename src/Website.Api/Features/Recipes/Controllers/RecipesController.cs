using Microsoft.AspNetCore.Mvc;
using Website.Api.Features.Recipes.Models;

using Microsoft.AspNetCore.Authorization;
using Website.Api.Features.Recipes.Services;

namespace Website.Api.Features.Recipes.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class RecipesController : ControllerBase
{
    private readonly IRecipeService _recipe;
    public RecipesController(IRecipeService RecipeService)
    {
        _recipe = RecipeService;
    }

    [HttpGet]
    public async Task<IList<Recipe>> GetBooks([FromQuery] RecipeQueryOptions opts)
    {
        return await _recipe.GetRecipes(opts);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRecipe(string id)
    {
        var recipe = await _recipe.GetRecipe(id);
        if (recipe is null)
            return NotFound();
        return Ok(recipe);
    }

    [HttpPost]
    public async Task<Recipe> CreateRecipe(RecipeContent content)
    {
        return await _recipe.CreateRecipe(content);
    }

    [HttpPut("{id}")]
    public async Task UpdateRecipe(string id, RecipeContent content)
    {
        await _recipe.UpdateRecipe(id, content);
    }
}
