using System;
using System.Collections.Generic;
using System.Text;

namespace Enterprise.Domain.Common
{
    public abstract class AuditableEntity : BaseEntity
    {
        public DateTime CreatedAtUtc { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }

        public string? UpdatedBy { get; set; }
    }
}
