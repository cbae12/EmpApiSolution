using Application.Contracts;
using Application.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.AccessControl;

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

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> LogUserIn(LoginDTO loginDTO)
        {
            var result = await user.LoginUserAsync(loginDTO);
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult<LoginResponse>> RegisterUser(RegisterUserDTO registerUserDTO)
        {
            var result = await user.RegisterUserAsync(registerUserDTO);
            return Ok(result);
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
        [Route("GET-admin")]
        public async Task<ActionResult<List<ApplicationUser>>> GetAsAdmin()
        {
            var result = await user.GetAsyncAdmin();
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("DELETE-admin")]
        public async Task<ActionResult<ServiceResponse>> DeleteUserAsync(int id)
        {
            var result = await user.DeleteUserAsync(id);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("ADDuser-admin")]
        public async Task<ActionResult<ServiceResponse>> AddUserAsync(ApplicationUser user)
        {
            var result = await this.user.AddUserAsync(user);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UPDATEUser-admin")]
        public async Task<ActionResult<ServiceResponse>> UpdateUserAsync(ApplicationUser user)
        {
            var result = await this.user.UpdateUserAsync(user);
            return Ok(result);
        }
    }
}
