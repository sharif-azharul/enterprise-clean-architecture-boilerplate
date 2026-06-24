using Enterprise.Application.Interfaces;
using Enterprise.Domain.Entities;
using Enterprise.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Enterprise.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(
            AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.Email == email);
        }
    }
}
