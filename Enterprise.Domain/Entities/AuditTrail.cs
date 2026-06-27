using Enterprise.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Enterprise.Domain.Entities
{
    public sealed class AuditTrail
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        public Guid Id { get; private set; } = Guid.NewGuid();

        /// <summary>
        /// Database table name.
        /// </summary>
        public string TableName { get; private set; } = string.Empty;

        /// <summary>
        /// Entity CLR type name.
        /// </summary>
        public string EntityName { get; private set; } = string.Empty;

        /// <summary>
        /// Create, Update or Delete.
        /// </summary>
        public AuditType AuditType { get; private set; }

        /// <summary>
        /// JSON representation of the entity primary key(s).
        /// Supports single, GUID and composite keys.
        /// </summary>
        public string KeyValues { get; private set; } = "{}";

        /// <summary>
        /// JSON representation of original values.
        /// Empty for Create operations.
        /// </summary>
        public string? OldValues { get; private set; }

        /// <summary>
        /// JSON representation of current values.
        /// Empty for Delete operations.
        /// </summary>
        public string? NewValues { get; private set; }

        /// <summary>
        /// User responsible for the change.
        /// </summary>
        public string? ChangedBy { get; private set; }

        /// <summary>
        /// UTC timestamp of the change.
        /// </summary>
        public DateTime ChangedAtUtc { get; private set; }

        // Required by EF Core
        private AuditTrail()
        {
        }

        public AuditTrail(
            string tableName,
            string entityName,
            AuditType auditType,
            string keyValues,
            string? oldValues,
            string? newValues,
            string? changedBy,
            DateTime changedAtUtc)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(tableName);
            ArgumentException.ThrowIfNullOrWhiteSpace(entityName);
            //ArgumentException.ThrowIfNullOrWhiteSpace(action);
            ArgumentException.ThrowIfNullOrWhiteSpace(keyValues);

            TableName = tableName;
            EntityName = entityName;
            AuditType = auditType;
            KeyValues = keyValues;
            OldValues = oldValues;
            NewValues = newValues;
            ChangedBy = changedBy;
            ChangedAtUtc = changedAtUtc;
        }
    }
}
