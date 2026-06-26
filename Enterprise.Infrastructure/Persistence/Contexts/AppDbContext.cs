using Enterprise.Application.Interfaces.Services;
using Enterprise.Domain.Common;
using Enterprise.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Enterprise.Infrastructure.Persistence.Contexts
{
    public class AppDbContext : DbContext
    {
        private readonly ICurrentUserService _currentUser;
        public AppDbContext(
            DbContextOptions<AppDbContext> options, ICurrentUserService currentUser)
            : base(options)
        {
            _currentUser = currentUser;
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();

        public DbSet<UserRole> UserRoles => Set<UserRole>();

        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(AppDbContext).Assembly);
        }
        public override async Task<int> SaveChangesAsync(
    CancellationToken cancellationToken = default)
        {
            ApplyAuditInformation();

            return await base.SaveChangesAsync(
                cancellationToken);
        }
        private void ApplyAuditInformation()
        {
            var entries =
                ChangeTracker
                    .Entries<AuditableEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAtUtc =
                        DateTime.UtcNow;

                    entry.Entity.CreatedBy =
                        _currentUser.Email ?? "System";
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAtUtc =
                        DateTime.UtcNow;

                    entry.Entity.UpdatedBy =
                        _currentUser.Email ?? "System";
                }
            }
        }
    }
}
