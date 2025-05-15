using Application.Contracts;
using Application.DTOs;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class TodoRepo : ITodo
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public TodoRepo(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<ServiceResponse> AddTodoAsync(TodoDTO todo, int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user is null)
            {
                return new ServiceResponse(false, "User not found");
            }
            Todo newTodo = new Todo()
            {
                UserId = userId,
                User = user,
                Title = todo.Title,
                Description = todo.Description,
                CreatedDate = todo.CreatedDate,
                DueDate = todo.DueDate,
                Status = todo.Status,
                UpdatedDate = todo.UpdatedDate
            };
            _context.Todos.Add(newTodo);
            user?.Todos?.Add(newTodo);
            await _context.SaveChangesAsync();
            return new ServiceResponse(true, "Added");
        }

        public async Task<ServiceResponse> DeleteTodoAsync(int id)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo is null)
            {
                return new ServiceResponse(false, "Todo not found");
            }

            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();
            return new ServiceResponse(true, "Deleted Todo");
        }

        public async Task<Todo> GetTodoAsync(int id)
        {
            var todo = _context.Todos.FirstOrDefault(s => s.Id == id);
            return todo;
        }

        public async Task<ICollection<Todo>> GetAllTodosAdmin()
        {
            var result = await _context.Todos.AsNoTracking().Include(u => u.User).ToListAsync();
            return result;
        }

        public async Task<ServiceResponse> UpdateTodoAsync(int id, TodoDTO todo)
        {
            var todoFound = _context.Todos.FirstOrDefault(s => s.Id == id);
            if (todoFound is null)
            {
                return new ServiceResponse(false, "Todo not found");
            }

            todoFound.Title = todo.Title;
            todoFound.Description = todo.Description;
            todoFound.CreatedDate = todo.CreatedDate;
            todoFound.UpdatedDate = todo.UpdatedDate;
            todoFound.DueDate = todo.DueDate;
            todoFound.Status = todo.Status;
            return new ServiceResponse(true, "Todo Updated");
        }
    }
}
