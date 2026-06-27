using Enterprise.Application.Interfaces.Services;
using Enterprise.Domain.Common;
using Enterprise.Domain.Entities;
using Enterprise.Domain.Enums;
using Enterprise.Infrastructure.Persistence.Audit;
using Enterprise.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Enterprise.Infrastructure.Persistence.Contexts
{
    public class AppDbContext : DbContext
    {
        private readonly ICurrentUserService _currentUser;
        private readonly AuditSaveChangesInterceptor _auditInterceptor;
        public AppDbContext(
            DbContextOptions<AppDbContext> options, ICurrentUserService currentUser, AuditSaveChangesInterceptor auditInterceptor)
            : base(options)
        {
            _currentUser = currentUser;
            _auditInterceptor = auditInterceptor;
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();

        public DbSet<UserRole> UserRoles => Set<UserRole>();

        //public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<AuditTrail> AuditTrails => Set<AuditTrail>();

        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
        //public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        //{
        //    //ApplyAuditInformation();

        //    //return await base.SaveChangesAsync(
        //    //    cancellationToken);
        //    ApplyAuditInformation();

        //    var auditLogs = BuildAuditLogs();

        //    var result = await base.SaveChangesAsync(cancellationToken);

        //    if (auditLogs.Any())
        //    {
        //        AuditLogs.AddRange(auditLogs);
        //        await base.SaveChangesAsync(cancellationToken);
        //    }

        //    return result;
        //}
        //private void ApplyAuditInformation()
        //{
        //    var entries = ChangeTracker.Entries<AuditableEntity>();

        //    foreach (var entry in entries)
        //    {
        //        if (entry.State == EntityState.Added)
        //        {
        //            entry.Entity.CreatedAtUtc = DateTime.UtcNow;
        //            entry.Entity.CreatedBy = _currentUser.Email ?? "System";
        //        }

        //        if (entry.State == EntityState.Modified)
        //        {
        //            entry.Entity.UpdatedAtUtc = DateTime.UtcNow;
        //            entry.Entity.UpdatedBy = _currentUser.Email ?? "System";
        //        }
        //    }
        //}

        //private List<AuditLog> BuildAuditLogs()
        //{
        //    ChangeTracker.DetectChanges();

        //    var auditLogs = new List<AuditLog>();

        //    foreach (var entry in ChangeTracker.Entries())
        //    {
        //        // Ignore unchanged entities
        //        if (entry.State == EntityState.Detached ||
        //            entry.State == EntityState.Unchanged)
        //        {
        //            continue;
        //        }

        //        // Don't audit audit logs
        //        if (entry.Entity is AuditLog)
        //        {
        //            continue;
        //        }

        //        var auditEntry = new AuditEntry(entry);

        //        auditEntry.TableName = entry.Metadata.GetTableName()!;

        //        switch (entry.State)
        //        {
        //            case EntityState.Added:
        //                auditEntry.AuditType = AuditType.Create;
        //                break;

        //            case EntityState.Modified:
        //                auditEntry.AuditType = AuditType.Update;
        //                break;

        //            case EntityState.Deleted:
        //                auditEntry.AuditType = AuditType.Delete;
        //                break;
        //        }
        //        var ignoredProperties = new[]
        //                {
        //                    "CreatedAtUtc",
        //                    "CreatedBy",
        //                    "UpdatedAtUtc",
        //                    "UpdatedBy"
        //                };
        //        foreach (var property in entry.Properties)
        //        {
        //            var propertyName = property.Metadata.Name;
        //            if (ignoredProperties.Contains(propertyName))
        //            {
        //                continue;
        //            }
        //            if (property.Metadata.IsPrimaryKey())
        //            {
        //                //auditEntry.RecordId = property.CurrentValue?.ToString() ?? "";

        //                continue;
        //            }

        //            switch (entry.State)
        //            {
        //                case EntityState.Added:

        //                    auditEntry.NewValues.Add(propertyName, property.CurrentValue);
        //                    break;

        //                case EntityState.Deleted:

        //                    auditEntry.OldValues.Add(propertyName, property.OriginalValue);
        //                    break;

        //                case EntityState.Modified:

        //                    if (!property.IsModified)
        //                        continue;

        //                    auditEntry.OldValues.Add(propertyName, property.OriginalValue);
        //                    auditEntry.NewValues.Add(propertyName, property.CurrentValue);

        //                    break;
        //            }
        //        }

        //        auditLogs.Add(CreateAuditLog(auditEntry));
        //    }

        //    return auditLogs;
        //}
        //private AuditLog CreateAuditLog(AuditEntry auditEntry)
        //{
        //    return new AuditLog
        //    {
        //        TableName = auditEntry.TableName,

        //        //RecordId = auditEntry.RecordId,

        //        AuditType =auditEntry.AuditType,

        //        OldValues = JsonSerializer.Serialize(auditEntry.OldValues),

        //        NewValues = JsonSerializer.Serialize(auditEntry.NewValues),

        //        ChangedBy = _currentUser.Email ?? "System",

        //        ChangedAtUtc = DateTime.UtcNow
        //    };
        //}
        //private List<AuditEntry> CaptureAuditEntries()
        //{
        //    ChangeTracker.DetectChanges();

        //    var auditEntries = new List<AuditEntry>();

        //    foreach (var entry in ChangeTracker.Entries())
        //    {
        //        // Ignore AuditLog table itself
        //        if (entry.Entity is AuditLog)
        //            continue;

        //        // Ignore unchanged and detached entities
        //        if (entry.State == EntityState.Detached ||
        //            entry.State == EntityState.Unchanged)
        //            continue;

        //        // Ignore entities that don't inherit from BaseEntity
        //        if (entry.Entity is not BaseEntity)
        //            continue;

        //        var auditEntry = new AuditEntry(entry)
        //        {
        //            TableName = entry.Metadata.GetTableName()!
        //        };

        //        auditEntry.AuditType = entry.State switch
        //        {
        //            EntityState.Added => AuditType.Create,
        //            EntityState.Modified => AuditType.Update,
        //            EntityState.Deleted => AuditType.Delete,
        //            _ => throw new NotSupportedException(
        //                $"Entity state '{entry.State}' is not supported.")
        //        };

        //        foreach (var property in entry.Properties)
        //        {
        //            var propertyName = property.Metadata.Name;

        //            // Ignore audit fields
        //            if (propertyName == nameof(AuditableEntity.CreatedAtUtc) ||
        //                propertyName == nameof(AuditableEntity.CreatedBy) ||
        //                propertyName == nameof(AuditableEntity.UpdatedAtUtc) ||
        //                propertyName == nameof(AuditableEntity.UpdatedBy))
        //            {
        //                continue;
        //            }

        //            // Database generated values (Identity, etc.)
        //            if (property.IsTemporary)
        //            {
        //                auditEntry.TemporaryProperties.Add(property);
        //                continue;
        //            }

        //            // Primary Key
        //            if (property.Metadata.IsPrimaryKey())
        //            {
        //                auditEntry.KeyValues[propertyName] =
        //                    property.CurrentValue!;

        //                continue;
        //            }

        //            switch (entry.State)
        //            {
        //                case EntityState.Added:

        //                    auditEntry.NewValues[propertyName] =
        //                        property.CurrentValue;

        //                    break;

        //                case EntityState.Deleted:

        //                    auditEntry.OldValues[propertyName] =
        //                        property.OriginalValue;

        //                    break;

        //                case EntityState.Modified:

        //                    if (!property.IsModified)
        //                        continue;

        //                    // Skip unchanged values
        //                    if (Equals(property.OriginalValue,
        //                               property.CurrentValue))
        //                        continue;

        //                    auditEntry.OldValues[propertyName] =
        //                        property.OriginalValue;

        //                    auditEntry.NewValues[propertyName] =
        //                        property.CurrentValue;

        //                    break;
        //            }
        //        }

        //        auditEntries.Add(auditEntry);
        //    }

        //    return auditEntries;
        //}
    }
}
