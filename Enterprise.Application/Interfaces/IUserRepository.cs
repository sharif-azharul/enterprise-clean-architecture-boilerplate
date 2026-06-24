using Enterprise.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Enterprise.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);

        Task AddAsync(User user);
    }
}
