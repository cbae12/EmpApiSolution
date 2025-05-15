using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts
{
    public interface ITodo
    {
        Task<ServiceResponse> AddTodoAsync(TodoDTO todo, int userId);
        Task<ServiceResponse> UpdateTodoAsync(int id, TodoDTO todo);
        Task<ServiceResponse> DeleteTodoAsync(int id);
        Task<Todo> GetTodoAsync(int id);
        Task<ICollection<Todo>> GetAllTodosAdmin();
    }
}
