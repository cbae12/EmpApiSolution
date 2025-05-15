using Application.Contracts;
using Application.DTOs;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
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
                string refreshToken = GenerateRefreshToken();
                return new LoginResponse(true, "Login successful", token, refreshToken);
            }
            else
            {
                return new LoginResponse(false, "Invalid Credentials");
            }
        }

        private string GenerateToken(string secretKey, string issuer, string audience,
            double expirationMinutes, IEnumerable<Claim> claims = null!)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(expirationMinutes),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateJWTToken(ApplicationUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var secretKey = _configuration["Jwt:Key"]!;
            var issuer = _configuration["Jwt:Issuer"]!;
            var audience = _configuration["Jwt:Audience"]!;
            var expirationMinutes = 72000;
            List<Claim> _claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.Name!),
                new Claim(ClaimTypes.Role, user.AccessRole!),
                new Claim(ClaimTypes.Hash, user.Password!)
            };

            return GenerateToken(secretKey, issuer, audience, expirationMinutes, _claims);
        }

        private string GenerateRefreshToken()
        {
            var secretKey = _configuration["Jwt:RefreshKey"]!;
            var expirationMinutes = Convert.ToDouble(_configuration["Jwt:RefreshTokenExpirationMinutes"]!);
            var issuer = _configuration["Jwt:Issuer"]!;
            var audience = _configuration["Jwt:Audience"]!;

            return GenerateToken(secretKey, issuer, audience, expirationMinutes);
        }

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

        //GET
        public async Task<List<ApplicationUser>> GetAsyncAdmin() => await _context.Users.AsNoTracking().ToListAsync();
        public async Task<List<UserDTO>> GetAsync()
        {
            var usersEntity = await _context.Users.AsNoTracking().ToListAsync();
            if (usersEntity == null)
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
            if (user == null)
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

        public async Task<ApplicationUser> GetByIdAdminAsync(int id)
        {
            ApplicationUser user = _context.Users.FirstOrDefault(e => e.Id == id);
            if (user == null)
            {
                return null;
            }
            return user;
        }

        //ADD USER
        public async Task<ServiceResponse> AddUserAsync(ApplicationUser user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            _context.Users.Add(user);
            await SaveChangesAsync();
            return new ServiceResponse(true, "Added");
        }

        //UPDATE
        public async Task<ServiceResponse> UpdateUserAsync(int id, ApplicationUser user)
        {
            var userFound = await _context.Users.FindAsync(id);
            userFound.Name = user.Name;
            userFound.Email = user.Email;
            userFound.AccessRole = user.AccessRole;
            userFound.Age = user.Age;
            userFound.Exp = user.Exp;
            userFound.Department = user.Department;
            userFound.Title = user.Title;
            userFound.Salary = user.Salary;
            _context.Update(userFound);
            await SaveChangesAsync();
            return new ServiceResponse(true, "Updated");
        }

        public async Task<ServiceResponse> UpdateUserDTOAsync(int id, UserDTO user)
        {
            ApplicationUser userB = await GetByIdAdminAsync(id);
            userB.Name = user.Name;
            userB.Email = user.Email;
            userB.Age = user.Age;
            userB.Salary = user.Salary;
            userB.Title = user.Title;
            userB.Exp = user.Exp;
            userB.Department = user.Department;
            _context.Update(userB);
            await SaveChangesAsync();
            return new ServiceResponse(true, "Updated");
        }

        //DELETE
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

        //Misc.
        private async Task SaveChangesAsync() => await _context.SaveChangesAsync();

        private async Task<ApplicationUser> FindUserByEmail(string email) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public Task SendEmailAsync(string email, string subject, string message)
        {
            string host = "jasonbae0120@gmail.com";
            string password = "jasonbae90";
            string name = "rebecca27@ethereal.email";
            string pw = "KHFbcAcgUtvKr6EfZn";

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(host, password)
            };

            return client.SendMailAsync(
                new MailMessage(from: name,
                                to: email,
                                subject,
                                message
                )
            );
        }

        //Todos - GET
        public async Task<ICollection<Todo>> GetTodos(int userId)
        {
            var result = await _context.Todos.Where(t => t.UserId == userId).OrderBy(t => t.Id).ToListAsync();
            var arr = new List<Todo>();
            for(int i = 0; i < result.Count; i++)
            {
                var todo = new Todo()
                {
                    Id = result[i].Id,
                    UserId = result[i].UserId,
                    User = result[i].User,
                    Title = result[i].Title,
                    Description = result[i].Description,
                    CreatedDate = result[i].CreatedDate,
                    UpdatedDate = result[i].UpdatedDate,
                    DueDate = result[i].DueDate,
                    Status = result[i].Status
                };
                arr.Add(todo);
            }
            return arr;
        }

        //Todos - ADD
        public async Task<ServiceResponse> AddTodoAsync(TodoDTO todo, int userId)
        {
            var result = await _context.Users.FindAsync(userId);
            Console.Write(result);
            if(result is null)
            {
                return new ServiceResponse(false, "User not found");
            }
            Todo newTodo = new Todo()
            {
                UserId = userId,
                User = result,
                Title = todo.Title,
                Description = todo.Description,
                DueDate = todo.DueDate,
                Status = todo.Status,
                CreatedDate = todo.CreatedDate,
                UpdatedDate = todo.UpdatedDate
            };
            if(newTodo.User == null)
            {
                return new ServiceResponse(false, "User not added");
            }
            _context.Todos.Add(newTodo);

            await SaveChangesAsync();
            return new ServiceResponse(true, "Todo Added");
        }

        //Todos - UPDATE
        public async Task<ServiceResponse> UpdateTodoAsync(int userId, int id, TodoDTO todo)
        {
            var result = await _context.Todos.Where(t => t.UserId == userId).ToListAsync();
            if(result is null)
            {
                return new ServiceResponse(false, "User not found");
            }
            var thisTodo = result.FirstOrDefault(t => t.Id == id);
            if(thisTodo is null)
            {
                return new ServiceResponse(false, "Todo not found");
            }
            thisTodo.Title = todo.Title;
            thisTodo.Description = todo.Description;
            thisTodo.CreatedDate = todo.CreatedDate;
            thisTodo.UpdatedDate = todo.UpdatedDate;
            thisTodo.DueDate = todo.DueDate;
            thisTodo.Status = todo.Status;
            _context.Update(thisTodo);
            await SaveChangesAsync();
            return new ServiceResponse(true, "Todo Updated");
        }

        //Todos - DELETE
        public async Task<ServiceResponse> DeleteTodoAsync(int userId, int id)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo is null)
            {
                return new ServiceResponse(false, "Todo not found");
            }
            _context.Todos.Attach(todo);
            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();
            return new ServiceResponse(true, "Deleted Todo");
        }
    }
}
