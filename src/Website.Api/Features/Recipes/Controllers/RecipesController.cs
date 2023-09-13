using Microsoft.AspNetCore.Mvc;
using Website.Api.Features.Recipes.Models;

using Microsoft.AspNetCore.Authorization;
using Website.Api.Features.Recipes.Services;
using Website.Api.Common;

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

    // [HttpGet]
    // public async Task<PagedData<Recipe>> GetRecipes([FromQuery] RecipeQueryOptions opts)
    // {
    //     return await _recipe.GetRecipes(opts);
    // }

    private readonly List<Recipe> _local = new List<Recipe>(){
        new() {Id = "1", Content = new() {Name = "A"}},
        new() {Id = "2", Content = new() {Name = "B"}},
        new() {Id = "3", Content = new() {Name = "C"}},
        new() {Id = "4", Content = new() {Name = "D"}},
        new() {Id = "5", Content = new() {Name = "E"}},
        new() {Id = "6", Content = new() {Name = "F"}},
        new() {Id = "7", Content = new() {Name = "G"}},
        new() {Id = "8", Content = new() {Name = "H"}},
        new() {Id = "9", Content = new() {Name = "I"}},
        new() {Id = "10", Content = new() {Name = "J"}},
    };

    [HttpGet]
    public async Task<PagedData<Recipe>> GetRecipes([FromQuery] RecipeQueryOptions opts)
    {
        Console.WriteLine(opts.PageNumber);
        Console.WriteLine(opts.PageSize);
        var data = _local.Skip(opts.PageSize * opts.PageNumber).Take(opts.PageSize).ToList();
        var nextPage = data.Count() < opts.PageSize ? 0 : opts.PageNumber + 1;
        return new PagedData<Recipe>(data, opts.PageNumber, opts.PageSize, nextPage);
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

    [HttpDelete("{id}")]
    public async Task DeleteRecipe(string id) => await _recipe.DeleteRecipe(id);
}
