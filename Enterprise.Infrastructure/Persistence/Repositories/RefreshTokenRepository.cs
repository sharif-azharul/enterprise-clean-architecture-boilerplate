using Enterprise.Application.Interfaces.Security;
using Enterprise.Domain.Entities;
using Enterprise.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Enterprise.Infrastructure.Persistence.Repositories
{
    public class RefreshTokenRepository
    : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(
            AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(
            RefreshToken refreshToken)
        {
            await _context.RefreshTokens
                .AddAsync(refreshToken);

            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken?>
            GetByTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(
                    x => x.Token == token);
        }

        public async Task UpdateAsync(
            RefreshToken refreshToken)
        {
            _context.RefreshTokens
                .Update(refreshToken);

            await _context.SaveChangesAsync();
        }
    }
}
