using Enterprise.Domain.Common;
using Enterprise.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Enterprise.Domain.Entities
{
    public class AuditLog : BaseEntity
    {
        public string TableName { get; set; } = string.Empty;

        public string RecordId { get; set; } = string.Empty;

        public AuditType AuditType { get; set; }

        public string? OldValues { get; set; }

        public string? NewValues { get; set; }

        public string? ChangedBy { get; set; }

        public DateTime ChangedAtUtc { get; set; }
    }
}
