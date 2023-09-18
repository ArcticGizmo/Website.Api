namespace Website.Api.Features.Todos.Services;

public class TodoDatabaseConfig
{
    public required string ConnectionString { get; set; }

    public required string Database { get; set; }
}