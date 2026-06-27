using System;
using System.Collections.Generic;
using System.Text;

namespace Enterprise.Infrastructure.Persistence.Audit
{
    /// <summary>
    /// Masks sensitive property values before they are persisted
    /// to the audit trail.
    /// </summary>
    internal static class AuditFieldMasker
    {
        private const string Mask = "********";

        private static readonly HashSet<string> SensitiveFields =
        [
            "Password",
        "PasswordHash",
        "RefreshToken",
        "SecurityStamp",
        "ConcurrencyStamp",
        "AccessToken",
        "IdToken",
        "ApiKey",
        "ApiSecret",
        "ClientSecret",
        "PrivateKey",
        "Secret",
        "Token"
        ];

        /// <summary>
        /// Returns the masked value when the property is considered sensitive;
        /// otherwise returns the original value.
        /// </summary>
        public static object? MaskValue(string propertyName, object? value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);

            if (value is null)
            {
                return null;
            }

            return IsSensitive(propertyName)
                ? Mask
                : value;
        }

        /// <summary>
        /// Determines whether a property should be masked.
        /// </summary>
        public static bool IsSensitive(string propertyName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);

            return SensitiveFields.Contains(propertyName);
        }
    }
}
