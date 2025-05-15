using Application.Contracts;
using Application.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly ITodo todo;
        public TodoController(ITodo todo)
        {
            this.todo = todo;
        }

        [HttpGet]
        [Route("GET")]
        public async Task<ActionResult<Todo>> GetTodo(int id)
        {
            Todo todo = await this.todo.GetTodoAsync(id);
            return todo;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("admin/GET/all")]
        public async Task<ActionResult<ICollection<Todo>>> GetAllTodos()
        {
            var result = await this.todo.GetAllTodosAdmin();
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("admin/ADD")]
        public async Task<ActionResult<ServiceResponse>> AddTodo(TodoDTO todo, int userId)
        {
            var result = await this.todo.AddTodoAsync(todo, userId);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("admin/DELETE")]
        public async Task<ActionResult<ServiceResponse>> DeleteTodo(int id)
        {
            var result = await this.todo.DeleteTodoAsync(id);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("admin/UPDATE/{id}")]
        public async Task<ActionResult<ServiceResponse>> UpdateTodo(int id, TodoDTO todo)
        {
            var result = await this.todo.UpdateTodoAsync(id, todo);
            return Ok(result);
        }
    }
}
