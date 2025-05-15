using Application.Contracts;
using Application.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.AccessControl;
using System.Security.Claims;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser user;
        public UserController(IUser user)
        {
            this.user = user;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> LogUserIn(LoginDTO loginDTO)
        {
            var result = await user.LoginUserAsync(loginDTO);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<LoginResponse>> RegisterUser(RegisterUserDTO registerUserDTO)
        {
            var result = await user.RegisterUserAsync(registerUserDTO);
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        [Route("current/GET")]
        public async Task<IActionResult> getCurrentUser()
        {
            int id = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await user.GetByIdAsync(id);
            return Ok(result);
        }

        [Authorize]
        [HttpPut]
        [Route("current/UPDATE")]
        public async Task<ActionResult<ServiceResponse>> editCurrentUser(UserDTO user)
        {
            int id = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var response = await this.user.UpdateUserDTOAsync(id, user);
            return Ok(response);
        }

        [Authorize]
        [HttpGet]
        [Route("GET")]
        public async Task<ActionResult<ServiceResponse>> GetUsers()
        {
            var result = await user.GetAsync();
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        [Route("GET/{id}")]
        public async Task<ActionResult<ServiceResponse>> GetByUserId(int id)
        {
            var result = await user.GetByIdAsync(id);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("admin/GET")]
        public async Task<ActionResult<List<ApplicationUser>>> GetAsAdmin()
        {
            var result = await user.GetAsyncAdmin();
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("admin/GET/{id}")]
        public async Task<ActionResult<List<ApplicationUser>>> GetByIdAdmin(int id)
        {
            var result = await user.GetByIdAdminAsync(id);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("admin/DELETE/{id}")]
        public async Task<ActionResult<ServiceResponse>> DeleteUserAsync(int id)
        {
            var result = await user.DeleteUserAsync(id);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("admin/ADD")]
        public async Task<ActionResult<ServiceResponse>> AddUserAsync(ApplicationUser user)
        {
            var result = await this.user.AddUserAsync(user);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("admin/UPDATE/{id}")]
        public async Task<ActionResult<ServiceResponse>> UpdateUserAsync(int id, ApplicationUser user)
        {
            var result = await this.user.UpdateUserAsync(id, user);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("Email/Confirm")]
        public async Task SendEmail(string body)
        {
            string email = HttpContext.User.FindFirstValue(ClaimTypes.Email)!;
            string subject = "Confirmation Email";
            string message = "You have created a new Account";
            await user.SendEmailAsync(email, subject, message);
        }

        [Authorize]
        [HttpGet("todos/GET")]
        public async Task<IActionResult> GetCurrentTodos()
        {
            int userId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var todos = await this.user.GetTodos(userId);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(todos);
        }

        [Authorize]
        [HttpPost("todo/ADD")]
        public async Task<ActionResult<ServiceResponse>> AddCUserTodo(TodoDTO todo)
        {
            int userId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await this.user.AddTodoAsync(todo, userId);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("todo/DELETE/{id}")]
        public async Task<ActionResult<ServiceResponse>> DeleteCUserTodo(int id)
        {
            int userId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await this.user.DeleteTodoAsync(userId, id);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("todo/UPDATE/{id}")]
        public async Task<ActionResult<ServiceResponse>> UpdateCUserTodo(int id, TodoDTO todo)
        {
            int userId = Convert.ToInt32(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await this.user.UpdateTodoAsync(userId, id, todo);
            return Ok(result);
        }
    }
}
