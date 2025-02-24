using Application.DTOs;
using Domain.Entities;

namespace Application.Contracts
{
    public interface IUser
    {
        Task<RegistrationResponse> RegisterUserAsync(RegisterUserDTO registerUserDTO);
        Task<LoginResponse> LoginUserAsync(LoginDTO loginDTO);
        Task<ServiceResponse> AddUserAsync(ApplicationUser user);
        Task<ServiceResponse> UpdateUserAsync(ApplicationUser user);
        Task<ServiceResponse> DeleteUserAsync(int id);
        Task<List<UserDTO>> GetAsync();
        Task<UserDTO> GetByIdAsync(int id);
        Task<List<ApplicationUser>> GetAsyncAdmin();
    }
}
