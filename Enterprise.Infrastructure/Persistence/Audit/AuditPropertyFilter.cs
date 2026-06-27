using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Text;

namespace Enterprise.Infrastructure.Persistence.Audit
{
    /// <summary>
    /// Provides helper methods for determining whether an EF Core property
    /// should be included in audit logging.
    /// </summary>
    internal static class AuditPropertyFilter
    {
        private static readonly HashSet<string> IgnoredProperties =
        [
            "CreatedAt",
        "CreatedAtUtc",
        "CreatedBy",
        "UpdatedAt",
        "UpdatedAtUtc",
        "UpdatedBy",
        "DeletedAt",
        "DeletedAtUtc",
        "DeletedBy"
        ];

        /// <summary>
        /// Determines whether the specified property should be audited.
        /// </summary>
        /// <param name="property">The EF Core property entry.</param>
        /// <returns>
        /// <c>true</c> if the property should be audited; otherwise, <c>false</c>.
        /// </returns>
        public static bool ShouldAudit(PropertyEntry property)
        {
            ArgumentNullException.ThrowIfNull(property);

            if (property.Metadata.IsShadowProperty())
            {
                return false;
            }

            if (property.Metadata.IsConcurrencyToken)
            {
                return false;
            }

            if (IgnoredProperties.Contains(property.Metadata.Name))
            {
                return false;
            }

            return true;
        }
    }
}
