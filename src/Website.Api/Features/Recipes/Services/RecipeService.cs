using Website.Api.Common;
using Website.Api.Features.Recipes.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Website.Api.Features.Recipes.Services;

public class RecipeService : IRecipeService
{
    const string RecipeCollection = "Recipes";

    private static Collation CollationEn = new("en", strength: CollationStrength.Primary);
    private readonly IMongoDatabase _db;
    private readonly IMongoCollection<RecipeDocument> _recipeCollection;

    public RecipeService(IOptions<RecipeDatabaseConfig> recipeDatabaseConfig)
    {
        var client = new MongoClient(recipeDatabaseConfig.Value.ConnectionString);

        _db = client.GetDatabase(recipeDatabaseConfig.Value.Database);

        _recipeCollection = _db.GetCollection<RecipeDocument>(RecipeCollection);
    }

    public async Task<PagedData<Recipe>> GetRecipes(RecipeQueryOptions opts)
    {
        IFindFluent<RecipeDocument, RecipeDocument> query;

        if (string.IsNullOrWhiteSpace(opts.SearchText))
        {
            query = _recipeCollection.Find(_ => true, new FindOptions { Collation = CollationEn });
        }
        else
        {
            var filter = GetRecipeTextFilter(opts.SearchText);
            query = _recipeCollection.Find(filter, new FindOptions { Collation = CollationEn });
        }


        var rawItems = await query.SortBy(opts).Paged(opts).ToListAsync();
        var items = rawItems.Select(d => d.ToRecipe()).ToList();

        int? nextPage = items.Count < opts.PageSize ? null : opts.PageNumber + 1;
        return new PagedData<Recipe>(items, opts.PageNumber, opts.PageSize, nextPage);
    }

    private FilterDefinition<RecipeDocument> GetRecipeTextFilter(string searchText)
    {
        var reg = new BsonRegularExpression(searchText, "i");
        var builder = Builders<RecipeDocument>.Filter;
        return builder.Regex("Name", reg) | builder.AnyEq("Tags", reg);
    }


    public async Task<Recipe?> GetRecipe(string recipeId)
    {
        if (!ObjectId.TryParse(recipeId, out _))
            return null;
        var doc = await _recipeCollection.Find(x => x.Id == recipeId).FirstOrDefaultAsync();
        return doc?.ToRecipe();
    }


    public async Task<Recipe> CreateRecipe(RecipeContent content)
    {
        var steps = content.Steps.Select(s => s.ToStepDocument()).ToList();

        var doc = new RecipeDocument
        {
            Name = content.Name,
            ImageUrl = content.ImageUrl,
            Time = content.Time,
            PeopleCount = content.PeopleCount,
            Ingredients = content.Ingredients,
            Tags = content.Tags,
            Steps = steps
        };

        await _recipeCollection.InsertOneAsync(doc);
        return doc.ToRecipe();
    }


    public async Task UpdateRecipe(string recipeId, RecipeContent content)
    {
        var doc = await _recipeCollection.Find(x => x.Id == recipeId).FirstAsync();
        doc.Name = content.Name;
        doc.ImageUrl = content.ImageUrl;
        doc.Time = content.Time;
        doc.PeopleCount = content.PeopleCount;
        doc.Tags = content.Tags;
        doc.Ingredients = content.Ingredients;
        doc.Steps = content.Steps.Select(s => s.ToStepDocument()).ToList();

        await _recipeCollection.ReplaceOneAsync(x => x.Id == recipeId, doc);
    }

    public async Task DeleteRecipe(string recipeId) => await _recipeCollection.DeleteOneAsync(x => x.Id == recipeId);

}


internal static class Extensions
{
    public static Recipe ToRecipe(this RecipeDocument doc)
    {
        return new Recipe()
        {
            Id = doc.Id!,
            Content = new()
            {
                Name = doc.Name,
                ImageUrl = doc.ImageUrl,
                PeopleCount = doc.PeopleCount,
                Ingredients = doc.Ingredients,
                Time = doc.Time,
                Tags = doc.Tags,
                Steps = doc.Steps.Select(s => s.ToStep()).ToList()
            }

        };
    }

    public static RecipeStepDocument ToStepDocument(this RecipeStep step)
    {
        return new RecipeStepDocument
        {
            Text = step.Text,
            ImageUrls = step.ImageUrls
        };
    }

    public static RecipeStep ToStep(this RecipeStepDocument step)
    {
        return new RecipeStep
        {
            Text = step.Text,
            ImageUrls = step.ImageUrls
        };
    }
}