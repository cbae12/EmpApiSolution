using Application.DTOs;
using Domain.Entities;

namespace Application.Contracts
{
    public interface IUser
    {
        //User
        Task<RegistrationResponse> RegisterUserAsync(RegisterUserDTO registerUserDTO);
        Task<LoginResponse> LoginUserAsync(LoginDTO loginDTO);
        Task<ServiceResponse> AddUserAsync(ApplicationUser user);
        Task<ServiceResponse> UpdateUserAsync(int id, ApplicationUser user);
        Task<ServiceResponse> UpdateUserDTOAsync(int id, UserDTO user);
        Task<ServiceResponse> DeleteUserAsync(int id);
        Task<List<UserDTO>> GetAsync();
        Task<UserDTO> GetByIdAsync(int id);
        Task<ApplicationUser> GetByIdAdminAsync(int id);
        Task<List<ApplicationUser>> GetAsyncAdmin();
        //Sending Email
        Task SendEmailAsync(string email, string subject, string message);
        //Todo CRUD
        Task<ICollection<Todo>> GetTodos(int id);
        Task<ServiceResponse> AddTodoAsync(TodoDTO todo, int userId);
        Task<ServiceResponse> UpdateTodoAsync(int userId, int id, TodoDTO todo);
        Task<ServiceResponse> DeleteTodoAsync(int userId, int id);
    }
}
