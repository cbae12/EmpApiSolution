using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Authentication
{
    public class AccountService : IAccount
    {
        private readonly HttpClient httpclient;
        public AccountService(HttpClient httpclient)
        {
            this.httpclient = httpclient;
        }
        public async Task<LoginResponse> LoginAccountAsync(LoginDTO model)
        {
            var response = await httpclient.PostAsJsonAsync("api/user/login", model);
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return result!;
        }

        public async Task<RegistrationResponse> RegisterAccountAsync(RegisterUserDTO model)
        {
            var response = await httpclient.PostAsJsonAsync("api/user/register", model);
            var result = await response.Content.ReadFromJsonAsync<RegistrationResponse>();
            return result!;
        }
    }
}
