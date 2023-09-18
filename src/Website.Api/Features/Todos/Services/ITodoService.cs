using Website.Api.Features.Todos.Models;

namespace Website.Api.Features.Todos.Services;

public interface ITodoService
{
    public Task<IReadOnlyList<Todo>> GetTodos();

    public Task<Todo> CreateTodo(string text);

    public Task UpdateTodo(string todoId, string text);

    public Task DeleteTodo(string todoId);
}