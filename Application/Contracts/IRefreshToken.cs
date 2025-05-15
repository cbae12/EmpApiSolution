using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts
{
    public interface IRefreshToken
    {
        Task<RefreshTokenDTO> GetByToken(string refreshToken);
        Task Create(RefreshTokenDTO refreshTokenDTO);
        Task DeleteByToken(string refreshToken);
        Task DeleteAll(int userId);
    }
}
