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
    public sealed class AuditSaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserService _currentUserService;

        private readonly List<AuditEntry> _pendingAuditEntries = [];

        private bool _isSavingAudit;

        public AuditSaveChangesInterceptor(
            ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            if (_isSavingAudit)
            {
                return base.SavingChanges(eventData, result);
            }

            var context = eventData.Context;

            if (context is null)
            {
                return base.SavingChanges(eventData, result);
            }

            ProcessSavingChanges(context);

            return base.SavingChanges(eventData, result);
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (_isSavingAudit)
            {
                return await base.SavingChangesAsync(
                    eventData,
                    result,
                    cancellationToken);
            }

            var context = eventData.Context;

            if (context is null)
            {
                return await base.SavingChangesAsync(
                    eventData,
                    result,
                    cancellationToken);
            }

            ProcessSavingChanges(context);

            return await base.SavingChangesAsync(
                eventData,
                result,
                cancellationToken);
        }

        private void ProcessSavingChanges(
            DbContext context)
        {
            context.ChangeTracker.DetectChanges();

            var auditEntries = CreateAuditEntries(context);

            if (auditEntries.Count == 0)
            {
                return;
            }

            context.Set<AuditTrail>()
                .AddRange(auditEntries.Select(x => x.ToAuditTrail()));
        }

        private List<AuditEntry> CreateAuditEntries(
            DbContext context)
        {
            var auditEntries = new List<AuditEntry>();

            var changedBy =
                _currentUserService.UserId ??
                "System";

            var changedAtUtc =
                DateTime.UtcNow;

            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (ShouldSkipEntry(entry))
                {
                    continue;
                }

                var auditEntry = new AuditEntry(entry)
                {
                    TableName = entry.Metadata.GetTableName() ?? string.Empty,
                    EntityName = entry.Metadata.ClrType.Name,
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

            return auditEntries;
        }
        private static bool ShouldSkipEntry(EntityEntry entry)
        {
            return entry.Entity is AuditTrail ||
                   entry.State is EntityState.Detached ||
                   entry.State is EntityState.Unchanged;
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
                        property.OriginalValue;

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

                auditEntry.ChangedColumns.Add(
                    property.Metadata.Name);

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

        public override int SavedChanges(
    SaveChangesCompletedEventData eventData,
    int result)
        {
            if (_isSavingAudit)
            {
                return base.SavedChanges(eventData, result);
            }

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
            if (_isSavingAudit)
            {
                return await base.SavedChangesAsync(
                    eventData,
                    result,
                    cancellationToken);
            }

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

            _isSavingAudit = true;

            try
            {
                foreach (var auditEntry in _pendingAuditEntries)
                {
                    UpdateTemporaryProperties(auditEntry);

                    context.Set<AuditTrail>()
                        .Add(auditEntry.ToAuditTrail());
                }

                _pendingAuditEntries.Clear();

                context.SaveChanges();
            }
            finally
            {
                _isSavingAudit = false;
            }
        }

        private async Task SavePendingAuditEntriesAsync(
            DbContext context,
            CancellationToken cancellationToken)
        {
            if (_pendingAuditEntries.Count == 0)
            {
                return;
            }

            _isSavingAudit = true;

            try
            {
                foreach (var auditEntry in _pendingAuditEntries)
                {
                    UpdateTemporaryProperties(auditEntry);

                    await context.Set<AuditTrail>()
                        .AddAsync(
                            auditEntry.ToAuditTrail(),
                            cancellationToken);
                }

                _pendingAuditEntries.Clear();

                await context.SaveChangesAsync(cancellationToken);
            }
            finally
            {
                _isSavingAudit = false;
            }
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
        public override void SaveChangesFailed(
    DbContextErrorEventData eventData)
        {
            _pendingAuditEntries.Clear();
            _isSavingAudit = false;

            base.SaveChangesFailed(eventData);
        }

        public override Task SaveChangesFailedAsync(
            DbContextErrorEventData eventData,
            CancellationToken cancellationToken = default)
        {
            _pendingAuditEntries.Clear();
            _isSavingAudit = false;

            return base.SaveChangesFailedAsync(
                eventData,
                cancellationToken);
        }
    }
}
