using Enterprise.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Enterprise.Application.Interfaces.Security
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken refreshToken);
        Task<RefreshToken?> GetByTokenAsync(string token);

        Task UpdateAsync(RefreshToken refreshToken);
    }
}
