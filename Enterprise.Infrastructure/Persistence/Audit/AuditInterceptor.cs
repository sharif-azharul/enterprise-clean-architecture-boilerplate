using Enterprise.Application.Interfaces.Services;
using Enterprise.Domain.Common;
using Enterprise.Domain.Entities;
using Enterprise.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Enterprise.Infrastructure.Persistence.Audit
{
    public sealed class AuditInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserService _currentUser;

        private List<AuditEntry>? _auditEntries;

        public AuditInterceptor(
            ICurrentUserService currentUser)
        {
            _currentUser = currentUser;
        }

        public override InterceptionResult<int> SavingChanges(
    DbContextEventData eventData,
    InterceptionResult<int> result)
        {
            if (eventData.Context != null)
            {
                _auditEntries =
                    CaptureAuditEntries(eventData.Context);
            }

            return base.SavingChanges(eventData, result);
        }
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
    DbContextEventData eventData,
    InterceptionResult<int> result,
    CancellationToken cancellationToken = default)
        {
            if (eventData.Context != null)
            {
                _auditEntries =
                    CaptureAuditEntries(eventData.Context);
            }

            return base.SavingChangesAsync(
                eventData,
                result,
                cancellationToken);
        }
        private List<AuditEntry> CaptureAuditEntries(
    DbContext context)
        {
            context.ChangeTracker.DetectChanges();

            var auditEntries = new List<AuditEntry>();

            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (entry.Entity is AuditLog)
                    continue;

                if (entry.State == EntityState.Detached)
                    continue;

                if (entry.State == EntityState.Unchanged)
                    continue;

                if (entry.Entity is not AuditableEntity)
                    continue;
                var auditEntry = new AuditEntry(entry)
                {
                    TableName =
        entry.Metadata.GetTableName()!
                };

                auditEntry.AuditType = entry.State switch
                {
                    EntityState.Added =>
                        AuditType.Create,

                    EntityState.Modified =>
                        AuditType.Update,

                    EntityState.Deleted =>
                        AuditType.Delete,

                    _ => throw new NotSupportedException()
                };

                foreach (var property in entry.Properties)
                {
                    if (ShouldIgnore(property))
                        continue;
                    if (property.IsTemporary)
                    {
                        auditEntry
                            .TemporaryProperties
                            .Add(property);

                        continue;
                    }
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[
                            property.Metadata.Name]
                                = property.CurrentValue!;

                        continue;
                    }
                    if (entry.State == EntityState.Added)
                    {
                        auditEntry.NewValues[
                            property.Metadata.Name]
                                = property.CurrentValue;

                        continue;
                    }
                    if (entry.State == EntityState.Deleted)
                    {
                        auditEntry.OldValues[
                            property.Metadata.Name]
                                = property.OriginalValue;

                        continue;
                    }
                    if (entry.State == EntityState.Modified)
                    {
                        if (!property.IsModified)
                            continue;

                        if (Equals(
                            property.OriginalValue,
                            property.CurrentValue))
                            continue;

                        auditEntry.OldValues[
                            property.Metadata.Name]
                                = property.OriginalValue;

                        auditEntry.NewValues[
                            property.Metadata.Name]
                                = property.CurrentValue;
                    }
                }

                if (entry.State == EntityState.Modified &&
    !auditEntry.NewValues.Any())
                {
                    continue;
                }

                auditEntries.Add(auditEntry);
            }

            return auditEntries;
        }

        private static bool ShouldIgnore(
    PropertyEntry property)
        {
            var name =
                property.Metadata.Name;

            return name is

                nameof(AuditableEntity.CreatedAtUtc)

                or nameof(AuditableEntity.CreatedBy)

                or nameof(AuditableEntity.UpdatedAtUtc)

                or nameof(AuditableEntity.UpdatedBy)

                or "ConcurrencyStamp"

                or "SecurityStamp";
        }





        //---------------


    }
}
