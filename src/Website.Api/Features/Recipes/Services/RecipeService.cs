using Website.Api.Common;
using Website.Api.Features.Recipes.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;


namespace Website.Api.Features.Recipes.Services;

public class RecipeService : IRecipeService
{
    const string RecipeCollection = "Recipes";

    private static Collation CollationEn = new Collation("en", strength: CollationStrength.Primary);
    private readonly IMongoDatabase _db;
    private readonly IMongoCollection<RecipeDocument> _recipeCollection;

    public RecipeService(IOptions<RecipeDatabaseConfig> recipeDatabaseConfig)
    {
        var client = new MongoClient(recipeDatabaseConfig.Value.ConnectionString);

        _db = client.GetDatabase(recipeDatabaseConfig.Value.Database);

        _recipeCollection = _db.GetCollection<RecipeDocument>(RecipeCollection);
    }

    public async Task<IList<Recipe>> GetRecipes(RecipeQueryOptions? opts = null)
    {
        IFindFluent<RecipeDocument, RecipeDocument> query;

        if (string.IsNullOrWhiteSpace(opts?.SearchText))
        {
            query = _recipeCollection.Find(_ => true, new FindOptions { Collation = CollationEn });
        }
        else
        {
            var filter = GetRecipeTextFilter(opts.SearchText);
            query = _recipeCollection.Find(filter, new FindOptions { Collation = CollationEn });
        }


        var items = await query.SortBy(opts).Paged(opts).ToListAsync();
        return items.Select(d => d.ToRecipe()).ToList();
    }

    private FilterDefinition<RecipeDocument> GetRecipeTextFilter(string searchText)
    {
        var reg = new BsonRegularExpression(searchText, "i");
        var builder = Builders<RecipeDocument>.Filter;
        return builder.Regex("name", reg) | builder.AnyEq("tags", reg);
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
        doc.Steps = content.Steps.Select(s => s.ToStepDocument()).ToList();
        throw new NotImplementedException();
    }
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
            ImageUrl = step.ImageUrl
        };
    }

    public static RecipeStep ToStep(this RecipeStepDocument step)
    {
        return new RecipeStep
        {
            Text = step.Text,
            ImageUrl = step.ImageUrl
        };
    }
}