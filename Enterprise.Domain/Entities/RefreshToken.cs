using Enterprise.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Enterprise.Domain.Entities
{
    public class RefreshToken :BaseEntity
    {
        public string Token { get; set; } = string.Empty;

        public DateTime ExpiresAtUtc { get; set; }

        public bool IsRevoked { get; set; }

        public Guid UserId { get; set; }

        public User User { get; set; } = null!;

        public string? CreatedByIp { get; set; }

        public string? RevokedByIp { get; set; }

        public DateTime? RevokedAtUtc { get; set; }

        public string? ReplacedByToken { get; set; }
    }
}
