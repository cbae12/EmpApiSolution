using Application.Contracts;
using Application.DTOs;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Repository
{
    public class UserRepo : IUser
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public UserRepo(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<LoginResponse> LoginUserAsync(LoginDTO loginDTO)
        {
            var getUser = await FindUserByEmail(loginDTO.Email!);
            if (getUser == null)
            {
                return new LoginResponse(false, "User not found");
            }

            bool checkPassword = BCrypt.Net.BCrypt.Verify(loginDTO.Password, getUser.Password);
            if (checkPassword)
            {
                string token = GenerateJWTToken(getUser);
                return new LoginResponse(true, "Login successful", token);
            }
            else
            {
                return new LoginResponse(false, "Invalid Credentials");
            }
        }

        private string GenerateJWTToken(ApplicationUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, user.AccessRole!)
            };
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: userClaims,
                expires: DateTime.Now.AddDays(5),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<ApplicationUser> FindUserByEmail(string email) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<RegistrationResponse> RegisterUserAsync(RegisterUserDTO registerUserDTO)
        {
            var getUser = await FindUserByEmail(registerUserDTO.Email!);
            if(getUser != null)
            {
                return new RegistrationResponse(false, "User already exist");
            }

            _context.Users.Add(new ApplicationUser()
            {
                Name = registerUserDTO.Name,
                Email = registerUserDTO.Email,
                AccessRole = registerUserDTO.AccessRole,
                Password = BCrypt.Net.BCrypt.HashPassword(registerUserDTO.Password)
            });
            await _context.SaveChangesAsync();
            return new RegistrationResponse(true, "Registration completed");
        }

        public async Task<ServiceResponse> AddUserAsync(ApplicationUser user)
        {
            _context.Users.Add(user);
            await SaveChangesAsync();
            return new ServiceResponse(true, "Added");
        }

        public async Task<ServiceResponse> UpdateUserAsync(ApplicationUser user)
        {
            _context.Update(user);
            await SaveChangesAsync();
            return new ServiceResponse(true, "Updated");
        }

        public async Task<ServiceResponse> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if(user == null)
            {
                return new ServiceResponse(false, "User not found");
            }
            _context.Users.Remove(user);
            await SaveChangesAsync();
            return new ServiceResponse(true, "Deleted");
        }
        public async Task<List<ApplicationUser>> GetAsyncAdmin() => await _context.Users.AsNoTracking().ToListAsync();
        public async Task<List<UserDTO>> GetAsync()
        {
            var usersEntity = await _context.Users.AsNoTracking().ToListAsync();
            if(usersEntity == null)
            {
                return null;
            }

            var users = from user in usersEntity
                        select new UserDTO()
                        {
                            Name = user.Name,
                            Email = user.Email,
                            Age = user.Age,
                            Salary = user.Salary,
                            Title = user.Title,
                            Exp = user.Exp,
                            Department = user.Department
                        };

            List<UserDTO> results = users.ToList();
            return results;
        }

        public async Task<UserDTO> GetByIdAsync(int id)
        {
            var user = _context.Users.FirstOrDefault(e => e.Id == id);
            if(user == null)
            {
                return null;
            }
            return new UserDTO
            {
                Name = user.Name,
                Email = user.Email,
                Age = user.Age,
                Salary = user.Salary,
                Title = user.Title,
                Exp = user.Exp,
                Department = user.Department
            };
        }

        private async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
