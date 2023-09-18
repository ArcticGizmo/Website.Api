using Microsoft.AspNetCore.Mvc;
using Website.Api.Features.Todos.Models;

using Microsoft.AspNetCore.Authorization;
using Website.Api.Features.Todos.Services;

namespace Website.Api.Features.Todos.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class TodosController : ControllerBase
{
    private readonly ITodoService _todo;
    public TodosController(ITodoService RecipeService)
    {
        _todo = RecipeService;
    }

    [HttpGet]
    public async Task<IReadOnlyList<Todo>> GetTodos()
    {
        return await _todo.GetTodos();
    }

    [HttpPost]
    public async Task<Todo> CreateTodo(CreateTodoReq req)
    {
        return await _todo.CreateTodo(req.Text);
    }

    [HttpPut("{id}")]
    public async Task UpdateTodo(string id, UpdateTodoReq req)
    {
        await _todo.UpdateTodo(id, req.Text);
    }

    [HttpDelete("{id}")]
    public async Task DeleteTodo(string id) => await _todo.DeleteTodo(id);
}

public record CreateTodoReq(string Text);
public record UpdateTodoReq(string Text);