using Enterprise.Application.Interfaces.Services;
using Enterprise.Domain.Entities;
using Enterprise.Domain.Enums;
using Enterprise.Infrastructure.Persistence.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Enterprise.Infrastructure.Persistence.Repositories
{
    internal sealed class AuditSaveChangesInterceptor_Back : SaveChangesInterceptor
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly List<AuditEntry> _pendingAuditEntries = [];

        public AuditSaveChangesInterceptor_Back(
            ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            var context = eventData.Context;

            if (context is null)
            {
                return base.SavingChanges(eventData, result);
            }

            ProcessAuditEntries(context);

            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;

            if (context is null)
            {
                return base.SavingChangesAsync(
                    eventData,
                    result,
                    cancellationToken);
            }

            ProcessAuditEntries(context);

            return base.SavingChangesAsync(
                eventData,
                result,
                cancellationToken);
        }

        private void ProcessAuditEntries(DbContext context)
        {
            context.ChangeTracker.DetectChanges();

            var auditEntries = CreateAuditEntries(context);

            if (auditEntries.Count == 0)
            {
                return;
            }

            var auditTrails = auditEntries
                .Select(x => x.ToAuditTrail())
                .ToList();

            context.Set<AuditTrail>()
                .AddRange(auditTrails);
        }

        private List<AuditEntry> CreateAuditEntries(
            DbContext context)
        {
            var auditEntries = new List<AuditEntry>();

            var changedBy =
                _currentUserService.UserId ??
                "System";

            var changedAtUtc = DateTime.UtcNow;

            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (ShouldSkipEntry(entry))
                {
                    continue;
                }

                var auditEntry = new AuditEntry(entry)
                {
                    TableName = entry.Metadata.GetTableName() ?? string.Empty,
                    EntityName = entry.Entity.GetType().Name,
                    ChangedBy = changedBy,
                    ChangedAtUtc = changedAtUtc
                };

                PopulateAuditEntry(
                    entry,
                    auditEntry);

                if (auditEntry.HasTemporaryProperties)
                {
                    _pendingAuditEntries.Add(auditEntry);
                }
                else
                {
                    auditEntries.Add(auditEntry);
                }
            }
            context.Set<AuditTrail>().AddRange(auditEntries.Select(x => x.ToAuditTrail()));
            return [];
        }


        private static bool ShouldSkipEntry(EntityEntry entry)
        {
            return entry.Entity is AuditTrail ||
                   entry.State is EntityState.Detached or EntityState.Unchanged;
        }

        private static void PopulateAuditEntry(
            EntityEntry entry,
            AuditEntry auditEntry)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    auditEntry.AuditType = AuditType.Create;
                    ProcessAddedEntity(entry, auditEntry);
                    break;

                case EntityState.Modified:
                    auditEntry.AuditType = AuditType.Update;
                    ProcessModifiedEntity(entry, auditEntry);
                    break;

                case EntityState.Deleted:
                    auditEntry.AuditType = AuditType.Delete;
                    ProcessDeletedEntity(entry, auditEntry);
                    break;
            }
        }

        private static void ProcessAddedEntity(
            EntityEntry entry,
            AuditEntry auditEntry)
        {
            foreach (var property in entry.Properties)
            {
                if (!AuditPropertyFilter.ShouldAudit(property))
                {
                    continue;
                }
                if (property.IsTemporary)
                {
                    auditEntry.TemporaryProperties.Add(property);
                    continue;
                }
                if (property.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyValues[property.Metadata.Name] =
                        property.CurrentValue;

                    continue;
                }

                auditEntry.NewValues[property.Metadata.Name] =
                    AuditFieldMasker.MaskValue(
                        property.Metadata.Name,
                        property.CurrentValue);
            }
        }

        private static void ProcessModifiedEntity(
            EntityEntry entry,
            AuditEntry auditEntry)
        {
            foreach (var property in entry.Properties)
            {
                if (!AuditPropertyFilter.ShouldAudit(property))
                {
                    continue;
                }

                if (property.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyValues[property.Metadata.Name] =
                        property.CurrentValue;

                    continue;
                }

                if (!property.IsModified)
                {
                    continue;
                }

                var originalValue =
                    AuditFieldMasker.MaskValue(
                        property.Metadata.Name,
                        property.OriginalValue);

                var currentValue =
                    AuditFieldMasker.MaskValue(
                        property.Metadata.Name,
                        property.CurrentValue);

                if (Equals(originalValue, currentValue))
                {
                    continue;
                }

                auditEntry.ChangedColumns.Add(property.Metadata.Name);

                auditEntry.OldValues[property.Metadata.Name] =
                    originalValue;

                auditEntry.NewValues[property.Metadata.Name] =
                    currentValue;
            }
        }

        private static void ProcessDeletedEntity(
            EntityEntry entry,
            AuditEntry auditEntry)
        {
            foreach (var property in entry.Properties)
            {
                if (!AuditPropertyFilter.ShouldAudit(property))
                {
                    continue;
                }

                if (property.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyValues[property.Metadata.Name] =
                        property.OriginalValue;

                    continue;
                }

                auditEntry.OldValues[property.Metadata.Name] =
                    AuditFieldMasker.MaskValue(
                        property.Metadata.Name,
                        property.OriginalValue);
            }
        }


    //    public override int SavedChanges(
    //SaveChangesCompletedEventData eventData,
    //int result)
    //    {
    //        // Complete pending audit entries here
    //        CompleteAuditEntries(eventData.Context);

    //        return base.SavedChanges(eventData, result);
    //    }
        public override int SavedChanges(
    SaveChangesCompletedEventData eventData,
    int result)
        {
            var context = eventData.Context;

            if (context is not null)
            {
                SavePendingAuditEntries(context);
            }

            return base.SavedChanges(eventData, result);
        }

        public override async ValueTask<int> SavedChangesAsync(
            SaveChangesCompletedEventData eventData,
            int result,
            CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;

            if (context is not null)
            {
                await SavePendingAuditEntriesAsync(
                    context,
                    cancellationToken);
            }

            return await base.SavedChangesAsync(
                eventData,
                result,
                cancellationToken);
        }



        private void SavePendingAuditEntries(
            DbContext context)
        {
            if (_pendingAuditEntries.Count == 0)
            {
                return;
            }

            foreach (var auditEntry in _pendingAuditEntries)
            {
                UpdateTemporaryProperties(auditEntry);

                context.Set<AuditTrail>()
                    .Add(auditEntry.ToAuditTrail());
            }

            _pendingAuditEntries.Clear();

            context.SaveChanges();
        }

        private async Task SavePendingAuditEntriesAsync(
            DbContext context,
            CancellationToken cancellationToken)
        {
            if (_pendingAuditEntries.Count == 0)
            {
                return;
            }

            foreach (var auditEntry in _pendingAuditEntries)
            {
                UpdateTemporaryProperties(auditEntry);

                await context
                    .Set<AuditTrail>()
                    .AddAsync(
                        auditEntry.ToAuditTrail(),
                        cancellationToken);
            }

            _pendingAuditEntries.Clear();

            await context.SaveChangesAsync(cancellationToken);
        }

        private static void UpdateTemporaryProperties(
            AuditEntry auditEntry)
        {
            foreach (var property in auditEntry.TemporaryProperties)
            {
                if (property.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyValues[property.Metadata.Name] =
                        property.CurrentValue;

                    continue;
                }

                auditEntry.NewValues[property.Metadata.Name] =
                    AuditFieldMasker.MaskValue(
                        property.Metadata.Name,
                        property.CurrentValue);
            }
        }
        private void CompleteAuditEntries(DbContext? context)
        {
            if (context is null)
            {
                return;
            }

            foreach (var auditEntry in _pendingAuditEntries)
            {
                foreach (var property in auditEntry.TemporaryProperties)
                {
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[property.Metadata.Name] =
                            property.CurrentValue;
                    }
                    else
                    {
                        auditEntry.NewValues[property.Metadata.Name] =
                            AuditFieldMasker.MaskValue(
                                property.Metadata.Name,
                                property.CurrentValue);
                    }
                }

                context.Set<AuditTrail>()
                       .Add(auditEntry.ToAuditTrail());
            }

            context.SaveChanges();

            _pendingAuditEntries.Clear();
        }
    }
}
