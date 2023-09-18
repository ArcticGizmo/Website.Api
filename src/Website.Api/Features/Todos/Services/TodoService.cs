using Website.Api.Common;
using Website.Api.Features.Todos.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Website.Api.Features.Todos.Services;

public class TodoService : ITodoService
{
    const string TodoCollection = "Todo";

    private static Collation CollationEn = new("en", strength: CollationStrength.Primary);
    private readonly IMongoDatabase _db;
    private readonly IMongoCollection<TodoDocument> _todoCollection;

    public TodoService(IOptions<TodoDatabaseConfig> todoDatabaseConfig)
    {
        var client = new MongoClient(todoDatabaseConfig.Value.ConnectionString);

        _db = client.GetDatabase(todoDatabaseConfig.Value.Database);

        _todoCollection = _db.GetCollection<TodoDocument>(TodoCollection);
    }

    public async Task<IReadOnlyList<Todo>> GetTodos()
    {
        var rawItems = await _todoCollection.Find(_ => true, new FindOptions { Collation = CollationEn }).ToListAsync();
        return rawItems.Select(r => r.ToTodo()).ToList();
    }

    public async Task<Todo> CreateTodo(string text)
    {
        var doc = new TodoDocument
        {
            Text = text
        };

        await _todoCollection.InsertOneAsync(doc);
        return doc.ToTodo();
    }

    public async Task UpdateTodo(string todoId, string text)
    {
        var doc = await _todoCollection.Find(x => x.Id == todoId).FirstAsync();
        doc.Text = text;

        await _todoCollection.ReplaceOneAsync(x => x.Id == todoId, doc);
    }

    public async Task DeleteTodo(string todoId) => await _todoCollection.DeleteOneAsync(x => x.Id == todoId);

}


internal static class Extensions
{
    public static Todo ToTodo(this TodoDocument doc)
    {
        return new Todo()
        {
            Id = doc.Id!,
            Text = doc.Text
        };
    }
}