using Enterprise.Domain.Entities;
using Enterprise.Domain.Enums;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Enterprise.Infrastructure.Persistence.Audit
{
    /// <summary>
    /// Represents an in-memory audit entry built from an EF Core EntityEntry
    /// before being converted into a persisted AuditTrail.
    /// </summary>
    internal sealed class AuditEntry
    {
        public AuditEntry(EntityEntry entry)
        {
            Entry = entry ?? throw new ArgumentNullException(nameof(entry));
        }

        /// <summary>
        /// The EF Core tracked entity.
        /// </summary>
        public EntityEntry Entry { get; }

        /// <summary>
        /// Database table name.
        /// </summary>
        public string TableName { get; set; } = string.Empty;

        /// <summary>
        /// CLR entity type name.
        /// </summary>
        public string EntityName { get; set; } = string.Empty;

        /// <summary>
        /// Audit operation.
        /// </summary>
        public AuditType AuditType { get; set; }

        /// <summary>
        /// Primary key values.
        /// Supports identity, GUID and composite keys.
        /// </summary>
        public Dictionary<string, object?> KeyValues { get; } = [];

        /// <summary>
        /// Original values before modification.
        /// </summary>
        public Dictionary<string, object?> OldValues { get; } = [];

        /// <summary>
        /// Current values after modification.
        /// </summary>
        public Dictionary<string, object?> NewValues { get; } = [];

        /// <summary>
        /// Modified property names.
        /// </summary>
        public List<string> ChangedColumns { get; } = [];

        /// <summary>
        /// User responsible for the change.
        /// </summary>
        public string? ChangedBy { get; set; }

        /// <summary>
        /// UTC timestamp of the change.
        /// </summary>
        public DateTime ChangedAtUtc { get; set; }
        public List<PropertyEntry> TemporaryProperties { get; } = [];

        /// <summary>
        /// Indicates whether the audit entry contains temporary properties
        /// that must be processed after SaveChanges().
        /// </summary>
        public bool HasTemporaryProperties => TemporaryProperties.Count > 0;
        /// <summary>
        /// Converts this temporary audit entry into a persistent AuditTrail entity.
        /// </summary>
        public AuditTrail ToAuditTrail()
        {
            return new AuditTrail(
                tableName: TableName,
                entityName: EntityName,
                auditType: AuditType,
                keyValues: Serialize(KeyValues),
                oldValues: OldValues.Count == 0 ? null : Serialize(OldValues),
                newValues: NewValues.Count == 0 ? null : Serialize(NewValues),
                changedBy: ChangedBy,
                changedAtUtc: ChangedAtUtc);
        }

        private static string Serialize(object value)
        {
            return JsonSerializer.Serialize(value);
        }
    }
}
